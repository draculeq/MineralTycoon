using System;
using System.Collections;
using System.Linq;
using Assets.Managers;
using UnityEngine;

namespace Assets.Buildings.Mockup
{
    public class BuildingMockup : MonoBehaviour, IDraggable
    {
        public static event Action MockupInitialized;
        public static event Action<BuildingMockup, GoogleSheet.Building> MockupConstructed;
        public static event Action MockupDestroyed;
        [SerializeField]
        private MeshRenderer _groundMesh;
        public IEnumerator _changingPosition;
        private Vector3 _position;
        public Vector3 Position
        {
            get { return _position; }
            private set
            {
                if (_position == value)
                    return;
                _position = value;
                if (PositionChanged != null) PositionChanged(_position);
            }
        }

        public event Action<Vector3> PositionChanged;

        private string _id;
        public BuildingMockup Init(string id)
        {
            _id = id;
            var model = Game.Database.Models.FirstOrDefault(a => a.Id == id).Model;
            if (model == null)
                return this;

            Instantiate(model, transform, false);
            transform.position = Map.ScreenToMapPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
            var position = Map.ScreenToFreeMapPosition(new Vector2(Screen.width / 2f, Screen.height / 2f)) ??
                           Map.ScreenToMapPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
            PlaceAtPosition(position);

            gameObject.AddComponent<Ground>().Init(this, _groundMesh);
            if (MockupInitialized != null) MockupInitialized();
            return this;
        }

        public void PlaceAtPosition(Vector3 position)
        {
            if (Position != position)
            {
                if (_changingPosition != null) StopCoroutine(_changingPosition);
                StartCoroutine(_changingPosition = ChangePosition(Position, position));
                Position = position;
            }
        }

        IEnumerator ChangePosition(Vector3 _source, Vector3 _target)
        {
            while (Vector3.Distance(transform.position, _target) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, _target, Time.deltaTime * 15);
                yield return null;
            }
        }

        public void ConfirmPurchase()
        {
            if (Game.Map.MapFields.Get((int)Position.x, (int)Position.z))
                DestroyMe(true);
        }

        public void AbortPurchase()
        {
            DestroyMe(false);
        }

        public void DestroyMe(bool success)
        {
            if (success)
            {
                if (MockupConstructed != null)
                    MockupConstructed(this, Game.Database.GetBuilding(_id));
            }
            if (MockupDestroyed != null) MockupDestroyed();
            Destroy(gameObject);
        }
    }
}
