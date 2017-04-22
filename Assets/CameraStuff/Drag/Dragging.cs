using UnityEngine;

namespace Assets.CameraStuff.Drag
{
    public class Dragging : DragState
    {
        private Vector3 _touchWorldPos;
        [SerializeField]
        private Vector2 _speed;

        public override DragState Init(DragStateMachine stateMachine)
        {
            base.Init(stateMachine);
            _touchWorldPos = GetWorldPoint(GetTouchPosition());
            return this;
        }

        public override DragState Tick()
        {
            if (!Touching())
                return _stateMachine.Get<Ready>().Init(_stateMachine);

            var worldDelta = GetWorldPoint(GetTouchPosition()) - _touchWorldPos;
            FindObjectOfType<CameraMovement>().Move(new Vector3(-worldDelta.x, 0, -worldDelta.z));

            return this;
        }

        private Vector3 GetWorldPoint(Vector2 screenPoint)
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(screenPoint), out hit);
            return hit.point;
        }

        private static bool Touching()
        {
#if UNITY_EDITOR
            return Input.GetMouseButton(0);
#else
            return Input.touchCount == 1;
#endif
        }

        private static Vector2 GetTouchPosition()
        {

#if UNITY_EDITOR
            return Input.mousePosition;
#else
            return Input.touches[0].position;
#endif
        }
    }
}
