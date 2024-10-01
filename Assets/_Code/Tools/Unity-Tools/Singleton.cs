using System;
using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// Traditional singleton pattern.
        /// Instantiates an instance if there isn't one in the scene.
        /// </summary>
        protected static T instance
        {
            get
            {
                if (Application.isPlaying && _instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        _instance = new GameObject(nameof(T)).AddComponent<T>();
                    }
                }

                return _instance;
            }
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
        /// If there is no instance, assigns this object,
        /// otherwise self destructs
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
