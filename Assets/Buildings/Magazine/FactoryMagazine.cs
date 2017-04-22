using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Buildings.Magazine
{
    [Serializable]
    public class FactoryMagazine
    {
        public Action<DateTime> ProductRemoved;
        [SerializeField]
        private ResourceMagazineData _product;
        [SerializeField]
        private List<ResourceMagazineData> _supplies;

        public ResourceMagazineData Product
        {
            get { return _product; }
        }
        public List<ResourceMagazineData> Supplies
        {
            get { return _supplies; }
        }
        #region product
        public int ProductMaxCapacity
        {
            get { return _product.MaxAmount; }
            set { _product.MaxAmount = Mathf.Clamp(value, 0, Int32.MaxValue); }
        }
        public int ProductCurrentCapacity
        {
            get { return _product.Amout; }
            set { _product.Amout = Mathf.Clamp(value, 0, ProductMaxCapacity); }
        }
        public int ProductFreeCapacity
        {
            get { return ProductMaxCapacity - ProductCurrentCapacity; }
        }
        public bool IsProductFull
        {
            get { return ProductCurrentCapacity >= ProductMaxCapacity; }
        }
        public bool HaveFreeProductCapacity(int amount)
        {
            return ProductFreeCapacity >= amount;
        }
        public void AddProduct(ResourceProduceData resource)
        {
            Debug.Assert(HaveFreeProductCapacity(resource.Amout));
            Debug.Assert(resource.ProductType == _product.ProductType);
            ProductCurrentCapacity += resource.Amout;
        }
        public void RemoveProduct(Resource resource, DateTime time)
        {
            Debug.Assert(HaveProduct(resource));
            _product.Amout -= resource.Amout;
            if (ProductRemoved != null) ProductRemoved(time);
        }
        private bool HaveProduct(Resource resource)
        {
            return ProductCurrentCapacity >= resource.Amout;
        }
        #endregion
        #region Supplies
        public bool HaveSupplies(IEnumerable<Resource> necessarySupplies)
        {
            foreach (var necessarySupply in necessarySupplies)
                if (_supplies.Any(a => a.ProductType == necessarySupply.ProductType && a.Amout < necessarySupply.Amout))
                    return false;
            return true;
        }
        public void TakeSupplies(IEnumerable<Resource> necessarySupplies)
        {
            Debug.Assert(HaveSupplies(necessarySupplies));

            foreach (var necessarySupply in necessarySupplies)
                _supplies.First(a => a.ProductType == necessarySupply.ProductType).Amout -= necessarySupply.Amout;
        }
        #endregion

        public void Init(ResourceMagazineData product, List<ResourceMagazineData> supplies)
        {
            _product = new ResourceMagazineData(product);
            _supplies = new List<ResourceMagazineData>();
            foreach (var supply in supplies)
                _supplies.Add(new ResourceMagazineData(supply));
        }

        public string GetData()
        {
            var str = string.Format("-- Magazine --\n" +
                                 "• Product: {0}\n" +
                                 "    Cap: {1}/{2}\n", Product.ProductType, ProductCurrentCapacity, ProductMaxCapacity); ;
            foreach (var supply in Supplies)
            {
                str += string.Format(
                    "• Supply: {0}\n" +
                    "    Cap: {1}/{2}\n", supply.ProductType, supply.Amout, supply.MaxAmount);
            }
            return str;

        }
    }
}
