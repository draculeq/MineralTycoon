using System;
using Assets.Buildings.TimeQueue;
using Assets.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Resources
{
    public class SellButton : MonoBehaviour
    {
        void Start()
        {
            FindObjectOfType<ProductionQueue>().Executed += Produced;
            Produced(null, DateTime.MaxValue);
        }

        private void Produced(IQueueExecutor queueExecutor, DateTime arg2)
        {
            GetComponentInChildren<Text>().text = "Sell for: " + Stock.GetStockValue();
        }

        public void Clicked()
        {
            FindObjectOfType<Stock>().SellAll();
            Produced(null, DateTime.MaxValue);
        }
    }
}
