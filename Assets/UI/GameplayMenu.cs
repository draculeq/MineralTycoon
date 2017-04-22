using Assets.Managers;
using Assets.UI.MenuStates;
using UnityEngine;

namespace Assets.UI
{
    public class GameplayMenu : MonoBehaviour
    {
        public void Save()
        {
            var state = FindObjectOfType<MenuStateMachine>().Current as MainScreen;
            if (state == null)
                return;
            state.Save();
        }

        public void Restart()
        {
            var state = FindObjectOfType<MenuStateMachine>().Current as MainScreen;
            if (state == null)
                return;
            state.Restart();
        }

        public void Rotate()
        {
            var state = FindObjectOfType<MenuStateMachine>().Current as MainScreen;
            if (state == null)
                return;
            state.Rotate();
        }
        public void ToggleDebugData(bool value)
        {
            Game.ShowDebugData = value;
        }
        public void ToggleUnlimitedCash(bool value)
        {
            Game.Wallet.Cash.UnlimitedCurrency = value;
        }
    }
}
