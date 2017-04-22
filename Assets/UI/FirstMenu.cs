using Assets.UI.MenuStates;
using Assets.UI.MenuStates.Intro;
using UnityEngine;

namespace Assets.UI
{
    public class FirstMenu : MonoBehaviour {

        public void Clear()
        {
            var state = FindObjectOfType<MenuStateMachine>().Current as IntroMenu;
            if (state == null)
                return;
            state.ClearStart();
        }

        public void Load()
        {
            var state = FindObjectOfType<MenuStateMachine>().Current as IntroMenu;
            if (state == null)
                return;
            state.Load();
        }
    }
}
