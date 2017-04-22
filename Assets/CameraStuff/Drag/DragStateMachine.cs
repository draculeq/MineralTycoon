using Assets.Plugins.Utilities;
using UnityEngine;

namespace Assets.CameraStuff.Drag
{
    public class DragStateMachine : StateMachine<DragState>
    {
        void Awake()
        {
            SetState(Get<Ready>().Init(this));
        }

        void Update()
        {
            Current = Current.Tick();
        }
    }
    public abstract class DragState : MonoBehaviour, IMachineState
    {
        protected DragStateMachine _stateMachine;

        protected virtual void OnEnable()
        {
            
        }
        public virtual DragState Init(DragStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            return this;
        }

        public abstract DragState Tick();
    }
}
