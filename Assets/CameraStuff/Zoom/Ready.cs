using UnityEngine;

namespace Assets.CameraStuff.Zoom
{
    public class Ready : ZoomState
    {
        public override ZoomState Init(ZoomStateMachine stateMachine)
        {
            base.Init(stateMachine);
            return this;
        }

        public override ZoomState Tick()
        {
            if (Mathf.Abs(Input.GetAxis("Zoom")) > 0)
                return _stateMachine.Get<Scrolling>().Init(_stateMachine);
            if (Input.touchCount == 2 && (Input.touches[0].phase == TouchPhase.Began || Input.touches[1].phase == TouchPhase.Began))
                return _stateMachine.Get<Pinching>().Init(_stateMachine);
            return this;
        }

        private static bool Zooming()
        {
            var value = false;

            return value;
        }
    }
}
