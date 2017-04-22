using System;
using System.Linq;
using Assets.Buildings.Magazine;
using UnityEngine;

namespace Assets.Managers
{
    public class Stock : MonoBehaviour
    {
        public void SellAll()
        {
            Game.Wallet.Cash.Amount += GetStockValue();
            foreach (var magazine in Game.Map.Factories.Select(a => a.Magazine))
            {
                Debug.Log("Sell " + magazine.Product.Amout + "x " + magazine.Product.ProductType + " value: " + ProductPrice(magazine));
                magazine.RemoveProduct(new Resource(magazine.Product.ProductType, magazine.Product.Amout), DateTime.UtcNow);
            }
        }

        public static int GetStockValue()
        {
            return Game.Map.Factories.Select(a => a.Magazine).Sum(magazine => ProductPrice(magazine));
        }

        private static int ProductPrice(FactoryMagazine magazine)
        {
            return magazine.Product.Amout * magazine.Product.ProductType.Id;
        }
    }
}
