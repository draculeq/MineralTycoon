using System;
using UnityEngine;

namespace Assets.CameraStuff.Zoom
{
    public class Scrolling : ZoomState
    {
        [SerializeField]
        private float _speed = 3;
        public override ZoomState Init(ZoomStateMachine stateMachine)
        {
            base.Init(stateMachine);
            FindObjectOfType<CameraMovement>().AddZoom(Input.GetAxis("Zoom") * _speed);
            return this;
        }

        public override ZoomState Tick()
        {
            if (Math.Abs(Input.GetAxis("Zoom") * _speed) == 0)
                return _stateMachine.Get<Ready>().Init(_stateMachine);

            FindObjectOfType<CameraMovement>().AddZoom(Input.GetAxis("Zoom") * _speed);

            return this;
        }
    }
}
