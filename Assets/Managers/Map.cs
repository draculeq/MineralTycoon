using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Buildings;
using Assets.Buildings.Factory;
using Assets.Buildings.Labolatory;
using Assets.Plugins.Utilities;
using UnityEngine;

namespace Assets.Managers
{
    public class Map : MonoBehaviour
    {
        public event Action Inited;
        [NonSerialized]
        public Constructor Constructor;
        private List<Building> _buildings = new List<Building>();
        [SerializeField]
        private List<Factory> _factories = new List<Factory>();
        [SerializeField]
        private List<Labolatory> _labolatories = new List<Labolatory>();
        private MapFields _mapFields;

        public MapFields MapFields
        {
            get { return _mapFields; }
        }
        public Dictionary<ProductType, List<Factory>> Producers = new Dictionary<ProductType, List<Factory>>();
        public Dictionary<ProductType, List<Factory>> Consumers = new Dictionary<ProductType, List<Factory>>();

        public IEnumerable<Building> Buildings
        {
            get { return _buildings.AsReadOnly(); }
        }
        public IEnumerable<Factory> Factories
        {
            get { return _factories.AsReadOnly(); }
        }
        public IEnumerable<Labolatory> Labolatories
        {
            get { return _labolatories.AsReadOnly(); }
        }

        private void Constructed(Building obj)
        {
            _buildings.Add(obj);
            _mapFields.Set((int)obj.Position.x, (int)obj.Position.z, Field.Availability.Blocked);
            if (obj is Factory)
            {
                var factory = obj as Factory;
                _factories.Add(factory);
                Producers[factory.ProduceData.ProductType].Add(factory);

                foreach (var necessarySupply in factory.NecessarySupplies)
                {
                    Consumers[necessarySupply.ProductType].Add(factory);
                }
            }
            if (obj is Labolatory)
            {
                UnityEngine.Debug.Log("LAB");
                var factory = obj as Labolatory;
                _labolatories.Add(factory);
            }
        }

        public void Init()
        {
            Constructor = GetComponentInChildren<Constructor>();
            Constructor.OnConstructed += Constructed;

            foreach (var productType in Database.Database.Products)
            {
                Producers.Add(productType, new List<Factory>());
                Consumers.Add(productType, new List<Factory>());
            }

            StartCoroutine(InitCoroutine());

        }
        private IEnumerator InitCoroutine()
        {
            for (var a = Save.LoadMapFields("map0"); a.MoveNext();)
            {
                UnityEngine.Debug.Log(a.Current);
                _mapFields = a.Current as MapFields;
                if (_mapFields != null)
                    break;
                yield return a.Current;
            }

            List<Building> cachedBuildings = new List<Building>();
            cachedBuildings.AddRange(Factories.Cast<Building>());
            cachedBuildings.AddRange(Labolatories.Cast<Building>());

            _factories = new List<Factory>();
            _labolatories = new List<Labolatory>();
            var stopwatch = new Stopwatch();

            var stopwatchAnalytics = new Stopwatch();
            stopwatchAnalytics.Start();
            //Create Buildings
            stopwatch.Start();
            foreach (var building in cachedBuildings)
            {
                Constructor.CreateBuilding(building.Position, Game.Database.GetBuilding(building.Id), building.ConstructionTime, building);
                if (stopwatch.ElapsedMilliseconds > Game.CoroutineLifetime)
                {
                    yield return null;
                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }
            DeadbitLog.Log("Creating buildings instances: " + stopwatchAnalytics.ElapsedMilliseconds + "ms", LogCategory.Analytics, LogPriority.Low);
            stopwatch.Reset();

            stopwatchAnalytics.Reset();
            stopwatchAnalytics.Start();

            //Initialize Buildings
            stopwatch.Start();
            for (var a = Constructor.Init(); a.MoveNext();)
            {
                if (stopwatch.ElapsedMilliseconds > Game.CoroutineLifetime)
                {
                    yield return null;
                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }
            DeadbitLog.Log("Initializing buildings: " + stopwatchAnalytics.ElapsedMilliseconds + "ms", LogCategory.Analytics, LogPriority.Low);

            Game.Transporter.Init();

            if (Inited != null) Inited();
            yield break;
        }
        public static Vector3? ScreenToFreeMapPosition(Vector2 screenPosition)
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out hit))
            {
                var newPos = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
                var closest =
                    Game.Map.MapFields.Fields.Where(a => a.Available == Field.Availability.Free)
                        .OrderBy(field => Vector2.Distance(new Vector2(field.X, field.Y), new Vector2(newPos.x, newPos.z)))
                        .First();
                return new Vector3(closest.X, 0, closest.Y);
            }
            return null;
        }
        public static Vector3 ScreenToMapPosition(Vector2 screenPosition)
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            Field finalField;
            if (Physics.Raycast(ray, out hit))
            {
                var newPos = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
                finalField = Game.Map.MapFields.Fields
                .OrderBy(field => Vector2.Distance(new Vector2(field.X, field.Y), new Vector2(newPos.x, newPos.z)))
                .First();
                return new Vector3(finalField.X, 0, finalField.Y);
            }
            throw new IndexOutOfRangeException("Sth is wrong woth Map fields!");
        }
    }
}
