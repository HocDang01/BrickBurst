using UnityEngine;

namespace CoreDang
{
    /// <summary>
    /// Singleton template
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Ins
        {
            get
            {
                if(_instance) return _instance;
                if(applicationIsQuitting) return _instance;
                _instance = (T)FindFirstObjectByType(typeof(T));
                if(!_instance) _instance = new GameObject("(Singleton) " + typeof(T).ToString()).AddComponent<T>();
                if(_instance.transform.parent == null) DontDestroyOnLoad(_instance.gameObject);
                return _instance;
            }
        }

        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        private static bool applicationIsQuitting = false;

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            if(!_instance)
            {
                _instance = gameObject.GetComponent<T>();
                if(_instance.transform.parent == null) DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}