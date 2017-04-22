using Assets.Plugins.Utilities;
using UnityEngine;

namespace Assets.CameraStuff.Zoom
{
    public class ZoomStateMachine : StateMachine<ZoomState>
    {
        void Awake()
        {
            Current = Get<Ready>().Init(this);
        }

        void Update()
        {
            Current = Current.Tick();
        }
    }

    public abstract class ZoomState : MonoBehaviour, IMachineState
    {
        protected ZoomStateMachine _stateMachine;

        public virtual ZoomState Init(ZoomStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            return this;
        }

        public abstract ZoomState Tick();
    }
}
