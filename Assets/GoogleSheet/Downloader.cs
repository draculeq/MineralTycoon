using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Plugins.Utilities;
using MiniJSON;
using UnityEngine;

namespace Assets.GoogleSheet
{
    public class Downloader : MonoBehaviour
    {
        public event Action<List<ProductType>> ProductsDownloaded;
        public event Action<List<Building>> BuildingsDownloaded;
        public event Action DownloadedAll;
#if UNITY_EDITOR
        private bool _downloadGoogleSheet;
#endif

        public void DownloadAll()
        {
            StartCoroutine(DownloadAllCoroutine());
        }
        private IEnumerator DownloadAllCoroutine()
        {
            for (var a = DownloadProducts(); a.MoveNext();)
                yield return a.Current;
            for (var a = DownloadBuildings(); a.MoveNext();)
                yield return a.Current;

            if (DownloadedAll != null) DownloadedAll();
            yield break;
        }

        #region Products
        private IEnumerator<List<ProductType>> DownloadProducts()
        {
            string jsonData = "";

            //Download Google Sheet
            for (var a = new GoogleSheetDownloader().DownloadProductsData(); a.MoveNext();)
            {
                if (a.Current != null)
                    jsonData = a.Current.Data;
                yield return null;
            }

            Debug.Assert(!string.IsNullOrEmpty(jsonData), "[Assert] Error when downloading Products from Google Sheet");

            //Download Offline Data
            if (string.IsNullOrEmpty(jsonData))
            {
                for (var a = new OfflineDownloader().DownloadProductsData(); a.MoveNext();)
                {
                    if (a.Current != null)
                        jsonData = a.Current.Data;
                    yield return null;
                }
            }
#if UNITY_EDITOR
            else
            {
                OfflineDownloader.SaveProductsData(jsonData);
            }
#endif

            Debug.Assert(!string.IsNullOrEmpty(jsonData), "[Assert] Error when reading Products from offline data");
            DeadbitLog.Log("[Products]\n" + jsonData, LogCategory.Database, LogPriority.Low);
            var products = DeserializeProducts(jsonData);
            yield return products;

            ShowProducts(products);
            if (ProductsDownloaded != null) ProductsDownloaded(products);
        }
        private static List<ProductType> DeserializeProducts(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                return new List<ProductType>();

            var products = new List<ProductType>();
            var rootData = Json.Deserialize(jsonData) as Dictionary<string, object>;
            var productsData = rootData["values"] as List<object>;
            foreach (var productData in productsData)
                products.Add(ProductType.Create(productData as List<object>));

            return products;
        }
        private static void ShowProducts(List<ProductType> products)
        {
            if (products.Count > 0)
                DeadbitLog.Log("Downloaded products: " + products.Select(a => a.Name.ToString()).Aggregate((a, b) => a + ", " + b), LogCategory.Database, LogPriority.Medium);
            else
                DeadbitLog.Log("No products downloaded", LogCategory.Database);
        }
        #endregion

        #region Buildings
        private IEnumerator<List<Building>> DownloadBuildings()
        {
            string jsonData = "";

            //Download Google Sheet
            for (var a = new GoogleSheetDownloader().DownloadBuildingsData(); a.MoveNext();)
            {
                if (a.Current != null)
                    jsonData = a.Current.Data;
                yield return null;
            }

            Debug.Assert(!string.IsNullOrEmpty(jsonData), "[Assert] Error when downloading Buildings from Google Sheet");

            //Download Offline Data
            if (string.IsNullOrEmpty(jsonData))
            {
                for (var a = new OfflineDownloader().DownloadBuildingsData(); a.MoveNext();)
                {
                    if (a.Current != null)
                        jsonData = a.Current.Data;
                    yield return null;
                }
            }
#if UNITY_EDITOR
            else
            {
                OfflineDownloader.SaveBuildingsData(jsonData);
            }
#endif

            Debug.Assert(!string.IsNullOrEmpty(jsonData), "[Assert] Error when reading Buildings from offline data");

            DeadbitLog.Log("[Buildings]\n" + jsonData, LogCategory.Database, LogPriority.Low);
            var buildings = DeserializeBuildings(jsonData);

            ShowBuildings(buildings);
            if (BuildingsDownloaded != null) BuildingsDownloaded(buildings);
        }
        private List<Building> DeserializeBuildings(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new List<Building>();

            var buildings = new List<Building>();
            var rootData = Json.Deserialize(json) as Dictionary<string, object>;
            var buildingsData = rootData["values"] as List<object>;
            foreach (var buildingData in buildingsData)
            {
                buildings.Add(Building.Create(buildingData as List<object>));
                StartCoroutine(DownloadIcon(buildings.Last()));
            }

            return buildings;
        }
        private static IEnumerator DownloadIcon(Building building)
        {
            var www = new WWW(building.iconUrl);
            yield return www;

            if (www.error != null)
            {
                building.Tex = new Texture2D(32, 32);
            }
            else
            {
                building.Tex = www.texture;
                building.OnTextureDownloaded();
            }
        }
        private static void ShowBuildings(List<Building> buildings)
        {
            if (buildings.Count > 0)
                DeadbitLog.Log("Downloaded buildings: " + buildings.Select(a => a.Id).Aggregate((a, b) => a + ", " + b) + "\n" + buildings.Select(a => a.ToString()).Aggregate((a, b) => a + "\n " + b), LogCategory.Database, LogPriority.Medium);
            else
                DeadbitLog.Log("No buildings downloaded", LogCategory.Database, LogPriority.Critical);
        }
        #endregion

    }
}