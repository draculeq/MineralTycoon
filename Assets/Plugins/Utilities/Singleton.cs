using UnityEngine;

namespace Assets.Plugins.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There are multiple singletons on the scene! (" + GetType() + ")", Instance);
                Destroy(this);
                return;
            }
            Instance = this as T;
        }
    }
}