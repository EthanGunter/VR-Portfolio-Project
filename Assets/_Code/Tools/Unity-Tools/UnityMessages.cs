using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SolarStorm.UnityToolkit
{
    public class UnityMessages : PersistentSingleton<UnityMessages>
    {
        public static event Action OnUpdate
        {
            add
            {
                Instance._updateEvent += value;
            }
            remove
            {
                Instance._updateEvent -= value;
            }
        }
        private event Action _updateEvent;
        public static event Action OnFixedUpdate
        {
            add
            {
                Instance._fixedUpdateEvent += value;
            }
            remove
            {
                Instance._fixedUpdateEvent -= value;
            }
        }
        private event Action _fixedUpdateEvent;

        [SerializeField] private UnityEvent _onUpdate;
        [SerializeField] private UnityEvent _onFixedUpdate;

        static IEnumerator _routine;
        public static void RestartCoroutine(IEnumerator routine)
        {
            if (_routine != null) Instance.StopCoroutine(_routine);
            _routine = routine;
            Instance.StartCoroutine(_routine);
        }

        private void Update()
        {
            _onUpdate?.Invoke();
            _updateEvent?.Invoke();
        }
        private void FixedUpdate()
        {
            _onFixedUpdate?.Invoke();
            _fixedUpdateEvent?.Invoke();
        }
    }
}
