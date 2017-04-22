using System;
using System.Linq;
using Assets.Buildings.TimeQueue;
using Assets.UI.MenuStates;
using Assets.UI.MenuStates.Intro;
using UnityEngine;

namespace Assets.UI.Resources
{
    public class ResourcesStatus : MonoBehaviour
    {
        [SerializeField]
        UnityEngine.UI.Text _text;

        void Start()
        {
            FindObjectOfType<ProductionQueue>().Executed += Produced;
            Game.Wallet.Cash.AmountChanged += OnCashChanged;
            FindObjectOfType<MenuStateMachine>().Get<StartingClearGame>().Disabled += GameLoaded;
            FindObjectOfType<MenuStateMachine>().Get<LoadingSavedGame>().Disabled += GameLoaded;
            Refresh();
        }

        private void GameLoaded()
        {
            Refresh();
        }

        private bool needUpdate = false;
        private void Produced(IQueueExecutor queueExecutor, DateTime arg2)
        {
            if (needUpdate == false)
            {
                needUpdate = true;
            }
        }

        private void OnCashChanged()
        {
            if (needUpdate == false)
                needUpdate = true;
        }

        void LateUpdate()
        {
            if (Game.ProductionQueue.OnTime && needUpdate)
            {
                Refresh();
                needUpdate = false;
            }
        }
        private void Refresh()
        {
            if (Database.Database.Products == null)
                return;

            var dic = Database.Database.Products.ToDictionary(productType => productType, productType => 0);

            foreach (var fac in Game.Map.Factories)
            {
                foreach (var supp in fac.Magazine.Supplies)
                {
                    if (!dic.ContainsKey(supp.ProductType))
                        dic.Add(supp.ProductType, 0);
                    dic[supp.ProductType] += supp.Amout;
                }
                if (!dic.ContainsKey(fac.Magazine.Product.ProductType))
                    dic.Add(fac.Magazine.Product.ProductType, 0);
                dic[fac.Magazine.Product.ProductType] += fac.Magazine.Product.Amout;
            }
            var cash = Game.Wallet.Cash.Amount == int.MaxValue ? "Unlimited" : Game.Wallet.Cash.Amount + "$";
            _text.text = dic.Aggregate("Cash: " + cash + "\n\nResources:\n", (current, prodKeyVal) => current + (prodKeyVal.Key.Name + " " + prodKeyVal.Value + "\n"));
        }
    }
}
