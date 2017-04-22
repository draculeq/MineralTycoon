using System.Collections.Generic;
using UnityEngine;

namespace Assets.Buildings
{
    public interface IProductionBuilding
    {
        Vector3 Position { get; }
        ResourceProduceData ProduceData { get; }
        List<Resource> Stock { get; }
        void RemoveProduct(Resource product);
        float Progress { get; }
    }
}
