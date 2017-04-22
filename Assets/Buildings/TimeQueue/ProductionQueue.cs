using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Plugins.Utilities;
using UnityEngine;

namespace Assets.Buildings.TimeQueue
{
    public class ProductionQueue : MonoBehaviour, IComparer<QueueRecord>
    {
        private List<QueueRecord> _records = new List<QueueRecord>();
        public event Action Inited;
        public event Action<IQueueExecutor, DateTime> Executed;
        public List<QueueRecord> Records
        {
            get { return _records; }
        }

        public bool OnTime
        {
            get { return Records.FirstOrDefault() == null || Records.FirstOrDefault().FinishTime > DateTime.UtcNow; }
        }


        public void Init()
        {
            StartCoroutine(InitCoroutine());
        }

        private IEnumerator InitCoroutine()
        {
            Game.Map.Constructor.OnConstructed += Constructed;

            var stopwatchAnalytics = new Stopwatch();
            stopwatchAnalytics.Start();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (var a = ProcessAll(); a.MoveNext();)
            {
                if (a.Current != null)
                    break;
                if (stopwatch.ElapsedMilliseconds > Game.CoroutineLifetime)
                {
                    yield return null;
                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }
            DeadbitLog.Log("Processing queue: " + stopwatchAnalytics.ElapsedMilliseconds + "ms", LogCategory.Analytics, LogPriority.Low);

            yield return null;
            StartCoroutine(ProcessAll());
            if (Inited != null) Inited();
            yield break;
        }

        private void Constructed(Building obj)
        {
            var executor = obj as IQueueExecutor;
            if (executor == null)
                return;
            executor.InitProduction(obj.ConstructionTime);
        }

        public void AddRecord(DateTime endTime, IQueueExecutor executor)
        {
            var index = _records.BinarySearch(new QueueRecord(endTime, executor), this);
            if (index < 0)
                index = ~index;
            _records.Insert(index, new QueueRecord(endTime, executor));
        }

        private IEnumerator ProcessAll()
        {
            yield return null;

            DateTime currentTime;
            var stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Start();
                currentTime = DateTime.UtcNow;
                while (_records.Any(a => a.FinishTime <= currentTime))
                {
                    ProduceSomething(_records.First());
                    if (stopwatch.ElapsedMilliseconds > Game.CoroutineLifetime)
                    {
                        yield return null;
                        stopwatch.Reset();
                        stopwatch.Start();
                    }
                }
                yield return 0;
            }
        }

        private void ProduceSomething(QueueRecord record)
        {
            ExecuteRecord(record);
            if (Executed != null) Executed(record.Executor, record.FinishTime);
        }

        private void ExecuteRecord(QueueRecord record)
        {
            _records.Remove(record);
            record.Executor.Execute(record.FinishTime);
        }

        private void BubbleSort(int[] array)
        {
            int temp = 0;
            for (int write = 0; write < array.Length; write++)
            {
                for (int sort = 0; sort < array.Length - 1; sort++)
                {
                    if (array[sort] > array[sort + 1])
                    {
                        temp = array[sort + 1];
                        array[sort + 1] = array[sort];
                        array[sort] = temp;
                    }
                }
            }
        }

        public int Compare(QueueRecord x, QueueRecord y)
        {
            if (x.FinishTime == y.FinishTime)
                return 0;
            if (x.FinishTime <= y.FinishTime)
                return -1;
            return 1;

        }
    }

    public class QueueRecord
    {
        public DateTime FinishTime;
        public IQueueExecutor Executor;

        public QueueRecord(DateTime finishTime, IQueueExecutor executor)
        {
            FinishTime = finishTime;
            Executor = executor;
        }


    }
}
