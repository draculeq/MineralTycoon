using UnityEngine;

namespace Assets.Buildings.Mockup
{
    public class FaceCamera : MonoBehaviour
    {
        private Transform _transform;
        private Transform _cam;
        void Start()
        {
            _transform = transform;
            _cam = Camera.main.transform;
        }

        void Update()
        {
            _transform.LookAt(_cam.localPosition);
            _transform.localRotation = Quaternion.Euler(new Vector3(-_transform.eulerAngles.x, 135, _transform.eulerAngles.z));
        }
    }
}
