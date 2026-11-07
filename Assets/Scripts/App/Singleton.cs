using UnityEngine;

namespace App
{
    public abstract class Singleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        protected static T _instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                
                lock (_lock)
                {

                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            var singleton = new GameObject("[SINGLETON] " + typeof(T));
                            _instance = singleton.AddComponent<T>();
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}