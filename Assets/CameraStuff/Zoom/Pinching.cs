using UnityEngine;

namespace Assets.CameraStuff.Zoom
{
    public class Pinching : ZoomState
    {
        private Vector2[] _touchPos = new Vector2[2];

        [SerializeField]
        private float _speed = 3;

        public override ZoomState Init(ZoomStateMachine stateMachine)
        {
            base.Init(stateMachine);
            _touchPos[0] = Input.touches[0].position;
            _touchPos[1] = Input.touches[1].position;
            return this;
        }

        public override ZoomState Tick()
        {
            if (Input.touchCount < 2)
                return _stateMachine.Get<Ready>().Init(_stateMachine);

            var diff =  Vector2.Distance(_touchPos[0], _touchPos[1]) -
                        Vector2.Distance(Input.touches[0].position, Input.touches[1].position)
                        ;
            diff /= Screen.width;
            

            _touchPos[0] = Input.touches[0].position;
            _touchPos[1] = Input.touches[1].position;

            FindObjectOfType<CameraMovement>().AddZoom(diff * _speed);

            return this;
        }

    }
}
