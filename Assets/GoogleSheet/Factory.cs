using System;
using System.Collections.Generic;

namespace Assets.GoogleSheet
{
    public class Factory : Building, IProductionBuilding
    {
        private readonly ProductType _productType;
        private readonly int _produceAmount;
        private readonly int _producePeriod;
        private readonly int _productMagazineCapacity;
        private readonly int _supplyStartIndex = 10;
        public List<Supply> Supplies;

        public ProductType ProductType { get { return _productType; } }
        public int ProduceAmount { get { return _produceAmount; } }
        public int ProducePeriod { get { return _producePeriod; } }
        public int ProductMagazineCapacity { get { return _productMagazineCapacity; } }

        public Factory(List<object> data) : base(data)
        {
            _productType = GetResourceType(data[6] as string);
            _produceAmount = Convert.ToInt32(data[7] as string);
            _producePeriod = Convert.ToInt32(data[8] as string);
            _productMagazineCapacity = Convert.ToInt32(data[9] as string);

            Supplies = new List<Supply>();
            for (int i = _supplyStartIndex; i < data.Count; i += 3)
            {
                var resourceType = GetResourceType(data[i] as string);
                var cost = Convert.ToInt32(data[i + 1]);
                var magazineCapacity = Convert.ToInt32(data[i + 2]);
                Supplies.Add(new Supply(resourceType, cost, magazineCapacity));
            }
        }

        public struct Supply
        {
            public ProductType ProductType;
            public int Cost;
            public int MagazineCapacity;

            public Supply(ProductType productType, int cost, int magazineCapacity)
            {
                ProductType = productType;
                Cost = cost;
                MagazineCapacity = magazineCapacity;
            }
        }
    }
}
