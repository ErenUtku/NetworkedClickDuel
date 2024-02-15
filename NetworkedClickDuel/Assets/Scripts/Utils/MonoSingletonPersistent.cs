using UnityEngine;

namespace Utils
{
    public class MonoSingletonPersistent<T> : MonoBehaviour
        where T : Component
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType((typeof(T))) as T;
                }

                return instance;
            }
        }

        public void Awake()
        {
            if (instance == null)
            {
                instance = FindObjectOfType((typeof(T))) as T;
                DontDestroyOnLoad(transform.root.gameObject);
            }
            else
            {
                Destroy(transform.root.gameObject);
            }
        }
    }
}