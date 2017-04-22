using Assets.Buildings.Mockup;
using UnityEngine;

namespace Assets.Enviroment.Islands
{
    public class Grid : MonoBehaviour {
        private Renderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.enabled = false;
            BuildingMockup.MockupInitialized += OnMockupInitialized;
            BuildingMockup.MockupDestroyed += OnMockupDestroyed;
        }

        void OnDestroy()
        {
            BuildingMockup.MockupInitialized -= OnMockupInitialized;
            BuildingMockup.MockupDestroyed -= OnMockupDestroyed;
        }

        private void OnMockupInitialized()
        {
            _renderer.enabled = true;
        }

        private void OnMockupDestroyed()
        {
            _renderer.enabled = false;
        }
    }
}
