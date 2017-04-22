using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class ProductType
    {
        public string Name { get { return _name; } }
        public int Id { get { return _id; } }
        [SerializeField]
        private string _name;
        [SerializeField]
        private int _id;

        public ProductType(ushort id)
        {
            Debug.Assert(id != 0, "Creating bad Product!");
            _id = id;
            _name = Database.Database.Products.FirstOrDefault(a => a._id == _id)._name;
        }
        public ProductType(ushort id, string name)
        {
            _id = id;
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        public static ProductType Create(List<object> data)
        {
            return new ProductType(Convert.ToUInt16(data[0].ToString()), data[1].ToString());
        }

        public static bool operator ==(ProductType a, ProductType b)
        {
            if (ReferenceEquals(null, b)) return false;
            if (ReferenceEquals(a, b)) return true;
            if (a.GetType() != b.GetType()) return false;
            return a.Equals(b);
        }

        public static bool operator !=(ProductType a, ProductType b)
        {
            return !(a == b);
        }
        protected bool Equals(ProductType other)
        {
            return _id == other._id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProductType)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }

    [Serializable]
    public class Resource
    {
        [SerializeField]
        private ProductType _productType;
        [SerializeField]
        private int _amout;

        public ProductType ProductType
        {
            get { return _productType; }
        }
        public int Amout
        {
            get { return _amout; }
            set
            {
                Debug.Assert(value >= 0);
                _amout = Mathf.Clamp(value, 0, int.MaxValue);
            }
        }

        public Resource(ProductType productType, int amount)
        {
            _productType = productType;
            _amout = amount;
        }

        public static bool operator ==(Resource a, Resource b)
        {
            if (ReferenceEquals(null, b)) return ReferenceEquals(a, null);
            if (ReferenceEquals(a, b)) return a.ProductType == b.ProductType;
            if (a.GetType() != b.GetType()) return false;
            return a.Equals(b);
        }
        public static bool operator !=(Resource a, Resource b)
        {
            return !(a == b);
        }
        protected bool Equals(Resource other)
        {
            return Equals(_productType.Id, other._productType.Id);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Resource)obj);
        }
        public override int GetHashCode()
        {
            return (_productType != null ? _productType.GetHashCode() : 0);
        }
    }
    [Serializable]
    public class ResourceMagazineData
    {
        [SerializeField]
        private Resource _resource;
        [SerializeField]
        private int _maxAmount;

        public int MaxAmount
        {
            get { return _maxAmount; }
            set { _maxAmount = value; }
        }
        public ProductType ProductType
        {
            get { return _resource.ProductType; }
        }
        public int Amout
        {
            get { return _resource.Amout; }
            set { _resource.Amout = value; }
        }


        public ResourceMagazineData(ProductType productType, int amount, int maxAmount)
        {
            Construct(productType, amount, maxAmount);
        }
        /// <summary>
        /// Copy Constructor
        /// </summary>
        public ResourceMagazineData(ResourceMagazineData product)
        {
            Construct(product.ProductType, product.Amout, product.MaxAmount);
        }

        private void Construct(ProductType productType, int amount, int maxAmount)
        {
            _resource = new Resource(productType, amount);
            MaxAmount = maxAmount;
        }
    }

    [Serializable]
    public class ResourceProduceData
    {
        [SerializeField]
        private Resource _resource;
        [SerializeField]
        private int _producePeriod;

        public int ProducePeriod
        {
            get { return _producePeriod; }
            set { _producePeriod = value; }
        }
        public ProductType ProductType
        {
            get { return _resource.ProductType; }
        }
        public int Amout
        {
            get { return _resource.Amout; }
            set { _resource.Amout = value; }
        }


        public ResourceProduceData(ProductType productType, int amount, int producePeriod)
        {
            Construct(productType, amount, producePeriod);
        }
        /// <summary>
        /// Copy Constructor
        /// </summary>
        public ResourceProduceData(ResourceProduceData product)
        {
            Construct(product.ProductType, product.Amout, product.ProducePeriod);
        }

        private void Construct(ProductType productType, int amount, int producePeriod)
        {
            _resource = new Resource(productType, amount);
            ProducePeriod = producePeriod;
        }
    }
}