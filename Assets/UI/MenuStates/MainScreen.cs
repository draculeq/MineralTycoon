using Assets.GoogleSheet;
using Assets.Plugins.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.UI.MenuStates
{
    public class MainScreen : MenuState
    {
        public void Save()
        {
            Managers.Save.SaveGame(Game.Map, Game.Wallet);
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Rotate()
        {
            var rotate = FindObjectOfType<Light>().GetComponent<Rotate>();
            rotate.enabled = !rotate.enabled;
        }
        public void ToggleDebugData(bool value)
        {
            Game.ShowDebugData = value;
        }

        public void Purchase(Building building)
        {
            StateMachine.SetState(StateMachine.Get<ConstructingBuilding>().Init(StateMachine));
            var a = Instantiate(Game.Database.Mockup);
            a.Init(building.Id);
        }
    }
}
