using UnityEngine;

namespace Utils
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Debug.Log($"Singleton {typeof(T).Name} already exists!");
                Destroy(gameObject);
            }
        }
    }
}