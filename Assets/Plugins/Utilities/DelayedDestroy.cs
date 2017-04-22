using UnityEngine;

namespace Assets.Plugins.Utilities
{
    public class DelayedDestroy : MonoBehaviour
    {
        [SerializeField]
        private float _delay;
        // Use this for initialization
        void Start()
        {
            Destroy(gameObject, _delay);
        }

    }
}
