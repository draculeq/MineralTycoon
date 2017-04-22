using System;
using Assets.Buildings;
using Assets.Managers;
using UnityEngine;

namespace Assets.CameraStuff.Drag
{
    public class DraggingBuilding : DragState
    {
        private IDraggable _draggable;
        public override DragState Init(DragStateMachine stateMachine)
        {
            throw new ApplicationException();
        }
        public virtual DragState Init(DragStateMachine stateMachine, IDraggable draggable)
        {
            base.Init(stateMachine);
            _draggable = draggable;
            return this;
        }

        public override DragState Tick()
        {
            if (!Input.GetMouseButton(0))
                return _stateMachine.Get<Ready>().Init(_stateMachine);
            var target = Map.ScreenToMapPosition(Input.mousePosition);
            _draggable.PlaceAtPosition(target);

            return this;
        }

    }
}
