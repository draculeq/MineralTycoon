using System.Linq;
using Assets.Buildings.Interfaces;
using UnityEngine;

namespace Assets.Buildings
{
    public class BuildingReferences : MonoBehaviour
    {
        [SerializeField]
        TextMesh _title;
        [SerializeField]
        private SpriteRenderer _progress;

        private IProgressable _progressable;
        private IDebugable _debugable;

        public IProgressable Progressable
        {
            get { return _progressable; }
        }

        public IDebugable Debugable
        {
            get { return _debugable; }
        }

        public TextMesh Title
        {
            get { return _title; }
        }

        public BuildingReferences Init(Building building)
        {
            _progressable = building as IProgressable;
            _debugable = building as IDebugable;

            Debug.Log(building.Id);
            var model = Game.Database.Models.FirstOrDefault(a => a.Id == building.Id).Model;
            if (model == null)
                return this;

            Instantiate(model, transform,false);
            transform.Rotate(new Vector3(0, 180, 0));

            return this;
        }
        void Update()
        {
            if (_progressable != null && (_progressable.Progress != _cachedProgress && (Mathf.Abs(_cachedProgress - _progressable.Progress) > 0.05f) || _progressable.Progress == 0))
                SetProductionProgress(_progressable.Progress);
        }
        void OnGUI()
        {
            if (Game.ShowDebugData)
            {
                var vec = Camera.main.WorldToScreenPoint(transform.position);
                vec = new Vector3(vec.x, Screen.height - vec.y, vec.z);
                GUI.Label(new Rect(vec, new Vector2(200, 200)), _debugable.GetDebugData(), Game.Instance.DebugBuildingGuiStyle);
            }
        }
        private float _cachedProgress;
        public void SetProductionProgress(float value)
        {
            _cachedProgress = value;
            _progress.transform.localScale = new Vector3(value * 10, _progress.transform.localScale.y, _progress.transform.localScale.z);
        }

    }
}
