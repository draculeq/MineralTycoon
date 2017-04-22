using System.Collections.Generic;
using Assets.Buildings.Mockup;
using Assets.UI.MenuStates;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.CameraStuff.Drag
{
    public class Ready : DragState
    {
        public override DragState Tick()
        {
            if (Touching() && !BlockedByUserInterface())
            {
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(GetTouchPosition()), out hit);

                if (FindObjectOfType<MenuStateMachine>().Current is ConstructingBuilding)
                {
                    var mockup = FindObjectOfType<BuildingMockup>();
                    if (Vector3.Distance(hit.point, mockup.transform.position) < 1)
                        return _stateMachine.Get<DraggingBuilding>().Init(_stateMachine, mockup);
                }
                return _stateMachine.Get<Dragging>().Init(_stateMachine);
            }
            return this;
        }

        private bool BlockedByUserInterface()
        {
            var results = new List<RaycastResult>();
            var pointer = new PointerEventData(EventSystem.current) { position = GetTouchPosition() };
            EventSystem.current.RaycastAll(pointer, results);
            return results.Count > 0;
        }

        private static bool Touching()
        {
#if UNITY_EDITOR
            return Input.GetMouseButtonDown(0);
#else
            return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
#endif
        }

        private Vector3 GetWorldPoint(Vector2 screenPoint)
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(screenPoint), out hit);
            return hit.point;
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
