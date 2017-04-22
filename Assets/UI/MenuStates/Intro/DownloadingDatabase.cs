namespace Assets.UI.MenuStates.Intro
{
    public class DownloadingDatabase : MenuState
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            Game.Database.RefreshAll();
            Game.Database.Refreshed += OpenMenu;
        }

        private void OpenMenu()
        {
            StateMachine.SetState(StateMachine.Get<IntroMenu>().Init(StateMachine));
        }
    }
}
