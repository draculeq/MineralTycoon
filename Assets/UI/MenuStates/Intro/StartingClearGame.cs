using Assets.Plugins.Utilities;

namespace Assets.UI.MenuStates.Intro
{
    public class StartingClearGame : MenuState
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            Game.Map.Inited += OnMapInited;
            Game.Map.Init();
        }

        private void OnMapInited()
        {
            Game.ProductionQueue.Inited += OnProductionQueueInited;
            Game.ProductionQueue.Init();
        }

        private void OnProductionQueueInited()
        {
            Game.Started = true;
            DeadbitLog.Log("Clear game started!", LogCategory.General, LogPriority.High);
            StateMachine.SetState(StateMachine.Get<MainScreen>().Init(StateMachine));
        }
    }
}
