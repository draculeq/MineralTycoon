namespace Assets.UI.MenuStates.Intro
{
    public class IntroMenu : MenuState
    {
        public void Load()
        {
            StateMachine.SetState(StateMachine.Get<LoadingSavedGame>().Init(StateMachine));
        }

        #region Clear start
        public void ClearStart()
        {
            StateMachine.SetState(StateMachine.Get<StartingClearGame>().Init(StateMachine));
        }
        #endregion
    }
}
