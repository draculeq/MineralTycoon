using DG.Tweening;
using UnityEngine;

namespace Assets.Buildings.Mockup
{
    public class Ground : MonoBehaviour
    {
        private BuildingMockup _mockup;
        private MeshRenderer _groundMesh;
        private Tweener _tween;
        private Color _currentColor;
        public void Init(BuildingMockup mockup, MeshRenderer groundMesh)
        {
            _mockup = mockup;
            _mockup.PositionChanged += OnPositionChanged;
            _groundMesh = groundMesh;
            OnPositionChanged(mockup.Position);
        }

        private void OnPositionChanged(Vector3 position)
        {
            var color = GetColor(position);
            if (color == _currentColor)
                return;

            _currentColor = color;
            if (_tween != null) _tween.Kill();
            _tween = _groundMesh.material.DOColor(_currentColor, 0.2f);
        }

        Color GetColor(Vector3 position)
        {
            return Game.Map.MapFields.Get((int)position.x, (int)position.z) ? Color.green : Color.red;
        }

        void OnDestroy()
        {
            _mockup.PositionChanged -= OnPositionChanged;
        }
    }
}
