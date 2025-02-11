using UnityEngine;

namespace Systems.SingletonPattern
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            // If there is already an instance, and it's not me, delete myself.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            transform.SetParent(null);  // Singletons need to be in the hierarchy root to work
            DontDestroyOnLoad(gameObject);
            Instance = (T)this;

            OnAwake();  // base.Awake();
        }

        // Overridable method to allow implementations to use Awake
        protected virtual void OnAwake() { }
    }
}