using UnityEngine;

namespace Assets.Plugins.Utilities
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _speed;
        // Update is called once per frame
        void Update()
        {
            transform.Rotate(_speed * Time.deltaTime,Space.World);
        }
    }
}
