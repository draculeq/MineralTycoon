using System;
using System.Collections.Generic;
using Assets.Plugins.Utilities;
using UnityEngine;

namespace Assets.GoogleSheet
{
    [Serializable]
    public abstract class Building //Remove abstract to see in inspector
    {
        public enum BuildingType
        {
            None,
            Laboratory,
            Factory
        };

        public string Id;
        public BuildingType Type;
        public string iconUrl;
        public Texture2D Tex;
        public int LvlNeed;
        public int Price;
        public event Action<Building, Texture2D> TextureDownloaded;

        public static Building Create(List<object> data)
        {
            var buildingType = (BuildingType)Convert.ToInt32(data[0].ToString());
            switch (buildingType)
            {
                case BuildingType.Factory:
                    return new Factory(data);
                case BuildingType.Laboratory:
                    return new Labolatory(data);
            }
            DeadbitLog.Log("Not defined building type!", LogCategory.Buildings, LogPriority.Critical);
            return null;
        }

        protected Building(List<object> data)
        {
            Type = (BuildingType)Convert.ToInt32(data[0].ToString());

            Id = data[1] as string;
            LvlNeed = Convert.ToInt32(data[2] as string);
            Price = Convert.ToInt32(data[3] as string);
            iconUrl = data[4] as string;
        }

        public override string ToString()
        {
            var supp = "";
            //foreach (var supply in Supplies)
            //    supp += supply.Value + " x " + supply.Key + ",\t";

            return "type: " + Type + "\n\t" + Id + "\n\t\tProduce: " + /*Produce +*/ "\n\t\tCosts: " + supp;
        }

        public void OnTextureDownloaded()
        {
            if (TextureDownloaded != null) TextureDownloaded(this, Tex);
        }

        protected ProductType GetResourceType(string type)
        {
            return new ProductType(Convert.ToUInt16(type));
        }
    }
}