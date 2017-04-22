using System.Linq;
using Assets.Plugins.Utilities;

namespace Assets.UI.MenuStates.Intro
{
    public class LoadingSavedGame : MenuState
    {
        public override MenuState Init(MenuStateMachine stateMachine)
        {
            base.Init(stateMachine);
            return this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Managers.Save.LoadSavedGame(Game.Map, Game.Wallet);
            Game.Map.Inited += MapInited;
            Game.Map.Init();
        }

        private void MapInited()
        {
            Game.ProductionQueue.Inited += ProductionQueueInited;
            Game.ProductionQueue.Init();
        }

        private void ProductionQueueInited()
        {
            Game.Started = true;
            DeadbitLog.Log("Game loaded! - " + Game.Map.Buildings.Count() + " buildings loaded!", LogCategory.General, LogPriority.Medium);
            StateMachine.SetState(StateMachine.Get<MainScreen>().Init(StateMachine));
        }
    }
}
