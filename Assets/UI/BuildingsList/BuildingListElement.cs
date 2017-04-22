using System;
using System.Linq;
using Assets.GoogleSheet;
using Assets.Managers;
using Assets.Plugins.Utilities;
using Assets.UI.MenuStates;
using UnityEngine;
using UnityEngine.UI;
using Building = Assets.GoogleSheet.Building;

namespace Assets.UI.BuildingsList
{
    public class BuildingListElement : MonoBehaviour
    {
        private Building _building;
        [SerializeField]
        private Text _name;
        [SerializeField]
        private Text _price;
        [SerializeField]
        private Text _produce;
        [SerializeField]
        private Text _produceSpeed;
        [SerializeField]
        private Image _buildingGfx;


        public BuildingListElement Init(Building building)
        {
            _building = building;
            _name.text = _building.Id;
            _price.text = _building.Price + "$";
            _produce.text = GetProduceText();
            _produceSpeed.text = GetProduceSpeed();

            if (_building.Tex != null)
            {
                _buildingGfx.color = new Color(1, 1, 1, 1);
                _buildingGfx.sprite = Sprite.Create(building.Tex, new Rect(0, 0, building.Tex.width, building.Tex.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                _buildingGfx.sprite = null;
                _buildingGfx.color = new Color(1, 1, 1, 0);
                _building.TextureDownloaded +=
                    (a, b) =>
                    {
                        _buildingGfx.color = new Color(1, 1, 1, 1);
                        _buildingGfx.sprite = Sprite.Create(building.Tex, new Rect(0, 0, building.Tex.width, building.Tex.height), new Vector2(0.5f, 0.5f));
                    };
            }

            return this;
        }

        private string GetProduceText()
        {
            var production = _building as IProductionBuilding;
            if (production != null)
                return production.ProductType.Name;

            var laboratory = _building as ILaboratoryBuilding;
            if (laboratory != null) return laboratory.ProductType.Name;
            throw new ArgumentOutOfRangeException();
        }

        private string GetProduceSpeed()
        {
            var production = _building as IProductionBuilding;
            if (production != null)
                return production.ProduceAmount + "u /" + production.ProducePeriod + "s";

            var laboratory = _building as ILaboratoryBuilding;
            return laboratory.ProduceAmount + "u /" + laboratory.ProducePeriod + "s";
        }

        public void OnBuy()
        {
            if (Game.Wallet.Cash.Amount < _building.Price)
            {
                Debug.Log("Not enought money");
                return;
            }

            if (Game.Map.MapFields.Fields.Any(a => a.Available == Field.Availability.Free))
            {
                DeadbitLog.Log("Buy", LogCategory.Buildings, LogPriority.Low);
                var menu = (FindObjectOfType<MenuStateMachine>().Current) as MainScreen;
                if (menu != null)
                {
                    menu.Purchase(_building);
                }
            }
            else
            {
                DeadbitLog.Log("No space on map", LogCategory.Buildings, LogPriority.High);
            }
        }
    }
}
