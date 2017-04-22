using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Buildings.Mockup;
using UnityEngine;

namespace Assets.Buildings
{
    public class Constructor : MonoBehaviour
    {
        [SerializeField]
        private BuildingReferences _buildingPrefab;

        private Transform _parent;

        public void Awake()
        {
            _parent = (GameObject.Find("Buildings") ?? new GameObject("Buildings")).GetComponent<Transform>();
            BuildingMockup.MockupConstructed += OnMockupConstructed;
        }

        void OnDestroy()
        {
            BuildingMockup.MockupConstructed -= OnMockupConstructed;
        }

        private void OnMockupConstructed(BuildingMockup buildingMockup, GoogleSheet.Building building)
        {
            Game.Wallet.Cash.Amount -= building.Price;
            Game.Map.Constructor.CreateBuilding(buildingMockup.Position, building, DateTime.UtcNow);
        }

        public event Action<Building> OnConstructed;

        public void CreateBuilding(Vector3 location, GoogleSheet.Building building, DateTime time, Building clonedBuilding = null)
        {
            var buildingReferences = Instantiate(_buildingPrefab);
            buildingReferences.name = building.Id;
            Building finalBuilding = null;
            Setup(buildingReferences, building, location);

            if (building is GoogleSheet.Factory)
                finalBuilding = CreateFactory(buildingReferences, (GoogleSheet.Factory)building, clonedBuilding as Factory.Factory);
            else if (building is GoogleSheet.Labolatory)
                finalBuilding = CreateLabolatory(buildingReferences, (GoogleSheet.Labolatory) building, clonedBuilding as Labolatory.Labolatory);
            else
                UnityEngine.Debug.LogAssertion("[Constructor] Unknown building type");

            buildingReferences.Init(finalBuilding);
            if (OnConstructed != null) OnConstructed(finalBuilding);
        }
        private static Factory.Factory CreateFactory(BuildingReferences buildingReferences, GoogleSheet.Factory fac, Factory.Factory clonedFactory)
        {

            var necessarySupplies = new List<Resource>();
            var suppliesMagazineData = new List<ResourceMagazineData>();

            foreach (var supply in fac.Supplies)
            {
                var supplyAmount = 0;
                if (clonedFactory != null)
                {
                    var clonedSupply = clonedFactory.Magazine.Supplies.FirstOrDefault(a => a.ProductType.Id == supply.ProductType.Id);
                    if (clonedSupply != null)
                        supplyAmount = clonedSupply.Amout;
                }

                necessarySupplies.Add(new Resource(supply.ProductType, supply.Cost));
                suppliesMagazineData.Add(new ResourceMagazineData(supply.ProductType, supplyAmount, supply.MagazineCapacity));
            }

            var productAmount = 0;
            if (clonedFactory != null)
                productAmount = clonedFactory.Magazine.Product.Amout;

            var constructionTime = DateTime.UtcNow;
            if (clonedFactory != null)
                constructionTime = clonedFactory.ConstructionTime;

            var productionEndDate = constructionTime;
            if (clonedFactory != null)
                productionEndDate = clonedFactory.ProductionEndDate;

            var factor = new Factory.Factory(
                fac.Id,
                buildingReferences.transform.position,
                constructionTime,
                productionEndDate,
                new ResourceProduceData(fac.ProductType, fac.ProduceAmount, fac.ProducePeriod),
                necessarySupplies,
                new ResourceMagazineData(fac.ProductType, productAmount, fac.ProductMagazineCapacity),
                suppliesMagazineData
                );

            return factor;
        }
        private static Labolatory.Labolatory CreateLabolatory(BuildingReferences buildingReferences, GoogleSheet.Labolatory lab, Labolatory.Labolatory clonedLabolatory)
        {
            var constructionTime = DateTime.UtcNow;
            if (clonedLabolatory != null)
                constructionTime = clonedLabolatory.ConstructionTime;

            var productionEndDate = constructionTime;
            if (clonedLabolatory != null)
                productionEndDate = clonedLabolatory.ProductionEndDate;

            var produceAmount = lab.ProduceAmount;
            if (clonedLabolatory != null)
                produceAmount = clonedLabolatory.ProduceAmount;

            var producePeriod = lab.ProducePeriod;
            if (clonedLabolatory != null)
                producePeriod = clonedLabolatory.ProducePeriod;

            var labolatory = new Labolatory.Labolatory(
                lab.Id,
                buildingReferences.transform.position,
                constructionTime,
                productionEndDate,
                produceAmount,
                producePeriod
                );

            return labolatory;
        }
        private static void Setup(BuildingReferences buildingReferences, GoogleSheet.Building building, Vector3 location)
        {
            buildingReferences.transform.parent = Game.Map.Constructor._parent;
            buildingReferences.transform.position = location;
            buildingReferences.Title.text = building.Id;
        }

        public IEnumerator Init()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var factory in Game.Map.Factories)
            {
                factory.InitProduction(factory.ProductionEndDate > DateTime.UtcNow ? DateTime.UtcNow : factory.ProductionEndDate);
                if (stopwatch.ElapsedMilliseconds > Game.CoroutineLifetime)
                {
                    yield return null;
                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }
            foreach (var labolatory in Game.Map.Labolatories)
            {
                labolatory.InitProduction(labolatory.ProductionEndDate > DateTime.UtcNow ? DateTime.UtcNow : labolatory.ProductionEndDate);
                if (stopwatch.ElapsedMilliseconds > Game.CoroutineLifetime)
                {
                    yield return null;
                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }
            yield break;
        }
    }
}
