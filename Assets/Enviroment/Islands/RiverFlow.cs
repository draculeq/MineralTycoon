using UnityEngine;

namespace Assets.Enviroment.Islands
{
    public class RiverFlow : MonoBehaviour
    {
        private MeshRenderer _renderer;
        [SerializeField]
        private Vector2 _speed;
        void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
        }
        void Update ()
        {
            _renderer.material.mainTextureOffset += _speed * Time.deltaTime;

        }
    }
}
