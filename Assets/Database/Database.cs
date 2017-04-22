using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assets.Buildings;
using Assets.Buildings.Mockup;
using Assets.GoogleSheet;
using Assets.Plugins.Utilities;
using UnityEngine;
using Building = Assets.GoogleSheet.Building;

namespace Assets.Database
{
    public class Database : Singleton<Database>
    {
        public event Action Refreshed;
        private Downloader _downloader;
        [SerializeField]
        private List<ProductType> _products=new List<ProductType>();
        [SerializeField]
        private List<Building> _buildings=new List<Building>();
        public static ReadOnlyCollection<ProductType> Products { get { return Instance._products.AsReadOnly(); } }
        public static ReadOnlyCollection<Building> Buildings { get { return Instance._buildings.AsReadOnly(); } }
        public List<BuildingModel> Models;
        public BuildingMockup Mockup;

        protected override void Awake()
        {
            base.Awake();
            _downloader = gameObject.AddComponent<Downloader>();
            _downloader.ProductsDownloaded += OnProductsDownloaded;
            _downloader.BuildingsDownloaded += OnBuildingsDownloaded;
            _downloader.DownloadedAll += OnDownloadedAll;
        }

        public void RefreshAll()
        {
            _downloader.DownloadAll();
        }
        private void OnProductsDownloaded(List<ProductType> products)
        {
            _products = products;
        }
        private void OnBuildingsDownloaded(List<Building> buildings)
        {
            _buildings = buildings;
        }
        private void OnDownloadedAll()
        {
            if (Refreshed != null) Refreshed();
        }

        public Building GetBuilding(string id)
        {
            return Buildings.FirstOrDefault(a => a.Id == id);
        }
        private void OnDestroy()
        {
            _downloader.ProductsDownloaded -= OnProductsDownloaded;
            _downloader.BuildingsDownloaded -= OnBuildingsDownloaded;
            _downloader.DownloadedAll -= OnDownloadedAll;
        }

        [Serializable]
        public class BuildingModel
        {
            public string Id;
            public GameObject Model;
        }
    }
}
