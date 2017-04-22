using System;
using System.Linq;
using Assets.Buildings;
using Assets.Buildings.Factory;
using Assets.Buildings.TimeQueue;
using UnityEngine;

namespace Assets.Managers
{
    public class Transporter : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer _lineRendererPrefab;

        public delegate void TransportedEventHandler(Building form, Building to, DateTime time);
        public event TransportedEventHandler Transported;
        void Start()
        {
            Game.ProductionQueue.Executed += Produced;
        }

        public void Init()
        {
            Game.Map.Constructor.OnConstructed += OnBuildingConstructed;
        }

        private void OnBuildingConstructed(Building obj)
        {
            TransportSuppliesToNewBuilding(obj);
        }

        private void TransportSuppliesToNewBuilding(Building building)
        {
            var factory = building as Factory;
            if (factory == null)
                return;
            if (factory.NecessarySupplies.Count == 0)
                return;

            ManyToOne(factory, factory.ConstructionTime);
        }

        private void Produced(IQueueExecutor queueExecutor, DateTime productionTime)
        {
            var factory = queueExecutor as Factory;

            if (factory == null)
                return;

            Debug.Assert(factory.Magazine.Product.Amout > 0);

            OneToMany(factory, productionTime);
            ManyToOne(factory, productionTime);
        }

        private void OneToMany(Factory giver, DateTime productionTime)
        {
            if (Game.Map.Consumers.ContainsKey(giver.ProduceData.ProductType))
            {
                var buildingsTo = Game.Map.Consumers[giver.ProduceData.ProductType]
                    .Where(a => a.NotFullSupplies.Any(b => b.ProductType == giver.ProduceData.ProductType))
                    .OrderBy(c => c.NotFullSupplies.First(d => d.ProductType == giver.ProduceData.ProductType).Amout + //Add progress * necessary
                    c.Progress * c.NecessarySupplies.First(d => d.ProductType == giver.ProduceData.ProductType).Amout);

                foreach (var buildingTo in buildingsTo)
                {
                    buildingTo.TransferAllPossibleProduct(
                        new Resource(giver.Magazine.Product.ProductType, giver.Magazine.Product.Amout), giver);
                    AddGfxLineRenderer(giver, buildingTo, productionTime);
                    if (Transported != null) Transported(giver, buildingTo, productionTime);
                    if (giver.Magazine.Product.Amout == 0) break;
                }
            }
        }

        private void ManyToOne(Factory reciver, DateTime time)
        {
            foreach (var necessarySupply in reciver.NecessarySupplies)
            {
                foreach (var fac in Game.Map.Producers[necessarySupply.ProductType].OrderByDescending(a => a.Magazine.Product.Amout))
                {
                    if (fac.Magazine.Product.Amout == 0 || reciver.NotFullSupplies.All(a => a.ProductType != necessarySupply.ProductType))
                        return;
                    reciver.TransferAllPossibleProduct(necessarySupply, fac);
                    AddGfxLineRenderer(fac, reciver, time);
                    if (Transported != null) Transported(fac, reciver, time);
                }
            }
        }

        private void AddGfxLineRenderer(Factory giver, Factory reciver, DateTime transportTime)
        {
            if (transportTime + TimeSpan.FromMilliseconds(1000) < DateTime.UtcNow)
                return;
            var a = Instantiate(_lineRendererPrefab);
            a.SetPositions(new[] { giver.Position, reciver.Position });
        }
    }
}