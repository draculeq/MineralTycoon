using System;
using System.Collections.Generic;

namespace Assets.GoogleSheet
{
    public class Labolatory : Building, ILaboratoryBuilding
    {
        private readonly int _produceAmount;
        private readonly int _producePeriod;
        private readonly ProductType _productType;

        public ProductType ProductType
        {
            get { return _productType; }
        }

        public int ProduceAmount
        {
            get { return _produceAmount; }
        }

        public int ProducePeriod
        {
            get { return _producePeriod; }
        }

        public Labolatory(List<object> data) : base(data)
        {
            _productType = GetResourceType(data[6] as string);
            _produceAmount = Convert.ToInt32(data[7] as string);
            _producePeriod = Convert.ToInt32(data[8] as string);
        }
    }
}
