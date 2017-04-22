using System;
using Assets.Buildings.Interfaces;
using Assets.Buildings.TimeQueue;
using Assets.Plugins.Utilities;
using UnityEngine;

namespace Assets.Buildings.Labolatory
{
    [Serializable]
    public class Labolatory : Building, IProgressable, IQueueExecutor
    {
        [SerializeField]
        private long _productionEndTimeSerialized;
        private DateTime _productionEndDate;
        [SerializeField]
        private int _produceAmount;
        [SerializeField]
        private int _producePeriod;

        public float Progress
        {
            get
            {
                return (float)(ProductionEndDate - DateTime.UtcNow).TotalMilliseconds / 1000 / ProducePeriod;
            }
        }

        public int ProduceAmount
        {
            get { return _produceAmount; }
            private set { _produceAmount = value; }
        }

        public int ProducePeriod
        {
            get { return _producePeriod; }
            private set { _producePeriod = value; }
        }

        public DateTime ProductionEndDate
        {
            get { return _productionEndDate; }
            set
            {
                _productionEndDate = value;
            }
        }

        private bool IsProducing(DateTime time)
        {
            return ProductionEndDate > time;
        }

        public bool CanProduce(DateTime time)
        {
            return !IsProducing(time);
        }

        public Labolatory(string id, Vector3 position, DateTime constructionTime, DateTime productionEndDate, int produceAmount, int producePeriod)
            : base(id, position, constructionTime)
        {
            ProductionEndDate = productionEndDate;
            ProduceAmount = produceAmount;
            ProducePeriod = producePeriod;
        }

        public void Execute(DateTime executionTime)
        {
            Game.Wallet.ResearchPoints.Amount += ProduceAmount;
            DeadbitLog.Log("[" + executionTime + "]" + " produced: Research Points in " + Id + " amount: " + ProduceAmount, LogCategory.Production, LogPriority.Low);
            if (CanProduce(executionTime))
                StartProducing(executionTime);
        }

        private void StartProducing(DateTime executionTime)
        {
            Debug.Assert(CanProduce(executionTime));
            ProductionEndDate = executionTime.Add(new TimeSpan(0, 0, ProducePeriod));
            Game.ProductionQueue.AddRecord(ProductionEndDate, this);
            DeadbitLog.Log("[" + executionTime + "], starting produce - End Date" + ProductionEndDate + " Research Points in " + Id + " amount: " + ProduceAmount, LogCategory.Production, LogPriority.Low);
        }

        public void InitProduction(DateTime dateTime)
        {
            if (CanProduce(dateTime))
                StartProducing(dateTime);
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            try
            {
                _productionEndTimeSerialized = ProductionEndDate.ToFileTimeUtc();
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            ProductionEndDate = DateTime.FromFileTimeUtc(_productionEndTimeSerialized);
        }
    }
}
