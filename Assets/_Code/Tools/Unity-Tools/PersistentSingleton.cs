using System;
using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
    {
        /// <summary>
        /// Traditional singleton pattern.
        /// Instantiates an instance if there isn't one in the scene.
        /// </summary>
        protected static T Instance
        {
            get
            {
                if (Application.isPlaying && _instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        Instance = new GameObject(typeof(T).Name + " Representative").AddComponent<T>();
                        Instance.Initialize();
                    }
                    _instance.Initialize();
                    MakePersistent(_instance.gameObject);
                }

                return _instance;
            }
            /*protected*/
            set
            {
                TimeStart = DateTime.Now;
                _instance = value;
            }
        }
        private static T _instance;

        public static DateTime TimeStart { get; private set; }
        public static TimeSpan TimeAlive => DateTime.Now - TimeStart;

        /// <summary>
        /// Ensures an object is not destroyed when a new scene loads
        /// </summary>
        private static void MakePersistent(GameObject obj)
        {
            // Free the object so Unity can move it to the DontDestroyOnLoad scene
            obj.transform.SetParent(null);
            DontDestroyOnLoad(obj);
        }

        protected virtual void Initialize()
        {
            MakePersistent(gameObject);
        }

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = (T)this;
                _instance.Initialize();
                MakePersistent(_instance.gameObject);
            } else
            {
                Destroy(this);
            }
        }
    }
}
