using Assets.GoogleSheet;
using Assets.UI.MenuStates;
using Assets.UI.MenuStates.Intro;
using UnityEngine;

namespace Assets.UI
{
    public class MenuRoot : MonoBehaviour
    {
        [SerializeField]
        private MenuStateMachine _menuStateMachine;

        public GameObject DownloadingDatabaseWindow;
        public GameObject MainMenuWindow;
        public GameObject GameplayScreen;

        public void Init()
        {
            DownloadingDatabaseWindow.SetActive(false);
            MainMenuWindow.SetActive(false);
            GameplayScreen.SetActive(false);

            _menuStateMachine.Init();

            _menuStateMachine.Get<IntroMenu>().Disabled += CloseMenu;
            _menuStateMachine.Get<IntroMenu>().Enabled += OpenMenu;
            _menuStateMachine.Get<MainScreen>().Enabled += OpenGameplay;
        }

        void OnDestroy()
        {
            if (_menuStateMachine!=null)
            {
                var intro = _menuStateMachine.Get<IntroMenu>();
                if (intro != null)
                {
                    intro.Disabled -= CloseMenu;
                    intro.Enabled -= OpenMenu;
                }

                var main = _menuStateMachine.Get<MainScreen>();
                if (main != null) main.Enabled -= OpenGameplay;
            }
        }

        void OpenMenu()
        {
            DownloadingDatabaseWindow.SetActive(false);
            MainMenuWindow.SetActive(true);
            GameplayScreen.SetActive(false);
        }

        private void CloseMenu()
        {
            MainMenuWindow.SetActive(false);
        }

        private void OpenWaiting()
        {
            DownloadingDatabaseWindow.SetActive(true);
            MainMenuWindow.SetActive(false);
            GameplayScreen.SetActive(false);
        }

        private void OpenGameplay()
        {
            GameplayScreen.SetActive(true);
            GameplayScreen.GetComponentInChildren<BuildingsList.BuildingsList>().Init();
        }

    }
}
