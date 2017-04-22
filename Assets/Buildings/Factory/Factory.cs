using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assets.Buildings.Interfaces;
using Assets.Buildings.Magazine;
using Assets.Buildings.TimeQueue;
using Assets.Plugins.Utilities;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Assets.Buildings.Factory
{
    [Serializable]
    [RequireComponent(typeof(FactoryMagazine))]
    public class Factory : Building, IProductionBuilding, IProgressable, IQueueExecutor
    {
        [SerializeField]
        private FactoryMagazine _magazine;
        private ResourceProduceData _produceData;
        private List<Resource> _necessarySupplies;


        public DateTime ProductionEndDate
        {
            get { return _productionEndDate; }
            set
            {
                _productionEndDate = value;
            }
        }
        private DateTime _productionEndDate;
        [SerializeField]
        private long _productionEndTimeSerialized;

        public FactoryMagazine Magazine
        {
            get { return _magazine; }
        }

        public ReadOnlyCollection<Resource> NecessarySupplies
        {
            get { return _necessarySupplies.AsReadOnly(); }
        }
        public List<Resource> NotFullSupplies
        {
            get
            {
                var notFullSupplies = new List<Resource>();
                foreach (var supply in Magazine.Supplies)
                {
                    if (supply.Amout < supply.MaxAmount)
                        notFullSupplies.Add(new Resource(supply.ProductType, supply.Amout));
                }
                foreach (var supply in NecessarySupplies)
                {
                    if (Magazine.Supplies.All(b => b.ProductType != supply.ProductType))
                        notFullSupplies.Add(new Resource(supply.ProductType, supply.Amout));
                }
                return notFullSupplies;
            }
        }
        protected override Sprite GetSprite()
        {
            return null;
        }

        public ResourceProduceData ProduceData
        {
            get { return _produceData; }
        }
        [Obsolete]
        public List<Resource> Stock
        {
            get
            {
                var stock = _magazine.Supplies.Select(supply => new Resource(supply.ProductType, supply.Amout)).ToList();
                stock.Add(new Resource(_magazine.Product.ProductType, _magazine.Product.Amout));

                return stock;
            }
        }

        internal void TransferAllPossibleProduct(Resource product, IProductionBuilding building)
        {
            var magazineSupply = _magazine.Supplies.FirstOrDefault(a => a.ProductType == product.ProductType);
            if (magazineSupply == null) return;
            var amountToAdd = new Resource(product.ProductType, Mathf.Clamp(product.Amout, 0, magazineSupply.MaxAmount - magazineSupply.Amout));
            magazineSupply.Amout += amountToAdd.Amout;
            product.Amout -= amountToAdd.Amout;
            building.RemoveProduct(amountToAdd);
        }
        public void RemoveProduct(Resource resource)
        {
            Debug.Assert(HaveProduct(resource));
            _magazine.ProductCurrentCapacity -= resource.Amout;
        }

        public float Progress
        {
            get
            {
                if (IsProducing(DateTime.UtcNow))
                    return (float)(ProductionEndDate - DateTime.UtcNow).TotalMilliseconds / 1000 / _produceData.ProducePeriod;
                return 0;
            }
        }

        private bool IsProducing(DateTime time)
        {
            return ProductionEndDate > time;
        }

        public bool CanProduce(DateTime time)
        {
            //Debug.Log(!IsProducing(time) + " " + Magazine.HaveFreeProductCapacity(ProduceData.Amout) + " " + Magazine.HaveSupplies(NecessarySupplies));
            return !IsProducing(time) && Magazine.HaveFreeProductCapacity(ProduceData.Amout) && Magazine.HaveSupplies(NecessarySupplies);
        }

        private bool HaveProduct(Resource resource)
        {
            return _magazine.ProductCurrentCapacity >= resource.Amout;
        }

        public Factory(string id, Vector3 position, DateTime constructionTime, DateTime productionEndDate, ResourceProduceData produceData, List<Resource> necessarySupplies, ResourceMagazineData product, List<ResourceMagazineData> supplies)
            : base(id, position, constructionTime)
        {
            _produceData = produceData;
            _necessarySupplies = necessarySupplies;
            _magazine = new FactoryMagazine();
            _magazine.Init(product, supplies);
            ProductionEndDate = productionEndDate;

            Game.Transporter.Transported += OnTransported;
            _magazine.ProductRemoved += OnProductRemoved;
        }

        void OnDestroy()
        {
            if (Game.Transporter != null) Game.Transporter.Transported -= OnTransported;
            if (_magazine != null) _magazine.ProductRemoved -= OnProductRemoved;
        }

        private void OnProductRemoved(DateTime time)
        {
            InitProduction(time);
        }

        public void InitProduction(DateTime time)
        {
            if (CanProduce(time))
                StartProducing(time);
        }

        private void OnTransported(Building buildingFrom, Building buildingTo, DateTime transportTime)
        {
            if (buildingTo == this || buildingFrom == this)
                if (CanProduce(transportTime))
                    StartProducing(transportTime);
        }

        public override string GetDebugData()
        {
            return string.Format("id: {0}\n" +
                          "Produce: {1}\n" +
                          "{2}u / {3}\n\n" +
                          "{4}", Id, ProduceData.ProductType, ProduceData.Amout, ProduceData.ProducePeriod,
                _magazine.GetData());
        }

        public void Execute(DateTime executionTime)
        {
            Magazine.AddProduct(ProduceData);
            if (CanProduce(executionTime))
            {
                DeadbitLog.Log("[" + executionTime + "]" + " produced: " + _produceData.ProductType + " in " + Id + " amount: " + Magazine.Product.Amout + "/" + Magazine.Product.MaxAmount, LogCategory.Production, LogPriority.Low);
                StartProducing(executionTime);
            }
        }

        private void StartProducing(DateTime executionTime)
        {
            Debug.Log("START");
            Debug.Assert(CanProduce(executionTime));
            Magazine.TakeSupplies(NecessarySupplies);
            ProductionEndDate = executionTime.Add(new TimeSpan(0, 0, _produceData.ProducePeriod));
            Game.ProductionQueue.AddRecord(ProductionEndDate, this);
            DeadbitLog.Log("[" + executionTime + "], starting produce - End Date" + ProductionEndDate + " " + _produceData.ProductType + " in " + Id + " amount: " + Magazine.Product.Amout + "/" + Magazine.Product.MaxAmount, LogCategory.Production, LogPriority.Low);
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
