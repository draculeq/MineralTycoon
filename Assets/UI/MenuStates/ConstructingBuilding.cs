using Assets.Buildings.Mockup;

namespace Assets.UI.MenuStates
{
    public class ConstructingBuilding : MenuState
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            BuildingMockup.MockupDestroyed += OnMockupDestroyed;
        }
        

        private void OnMockupDestroyed()
        {
            StateMachine.SetState(StateMachine.Get<MainScreen>().Init(StateMachine));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BuildingMockup.MockupDestroyed -= OnMockupDestroyed;
        }
    }
}
