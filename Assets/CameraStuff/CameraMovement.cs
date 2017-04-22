using Assets.Plugins.Utilities;
using UnityEngine;

namespace Assets.CameraStuff
{
    public class CameraMovement : MonoBehaviour
    {

        [SerializeField]
        private float _speed = 20;
        private float _minZoom = 0.6f;
        [SerializeField]
        private float _maxZoom = 6f;

        private float _targetZoom;

        void Awake()
        {
            ClampZoom();
            _targetZoom = transform.position.y;
        }
        void Update()
        {
            Move(
                new Vector3(
                    Input.GetAxis("Horizontal") * _speed * Time.deltaTime,
                    0,
                    Input.GetAxis("Vertical") * _speed * Time.deltaTime
                    )
                );
            transform.position = new Vector3(
                transform.position.x,
                Mathf.Lerp(transform.position.y, _targetZoom, Time.deltaTime*10),
                transform.position.z);
            SetCameraZoomAngle();

        }

        public void Move(Vector3 diff)
        {
            transform.Translate(
                diff.x * new Vector3(0.5f, 0, 0.5f)
                + diff.z * new Vector3(-0.5f, 0, 0.5f), Space.World);
        }

        public void AddZoom(float zoomAmount)
        {
            _targetZoom = _targetZoom + zoomAmount * Time.deltaTime;
            ClampZoom();
        }

        private void ClampZoom()
        {
            if (transform.position.y < _minZoom)
                transform.position = new Vector3(transform.position.x, _minZoom, transform.position.z);
            if (transform.position.y > _maxZoom)
                transform.position = new Vector3(transform.position.x, _maxZoom, transform.position.z);
            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
        }

        private void SetCameraZoomAngle()
        {
            if (transform.position.y > 2)
                transform.localRotation = Quaternion.Euler(transform.position.y.MapClamped(_maxZoom, 2, 60, 45),
                    transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
            else
                transform.localRotation = Quaternion.Euler(transform.position.y.MapClamped(_minZoom, 2, 35, 45),
                    transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
        }
    }
}
