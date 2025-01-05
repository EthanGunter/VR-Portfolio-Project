
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SolarStorm.UnityToolkit
{
    // Concrete Objects
    [CreateAssetMenu(menuName = "Events/Float Event")]
    public class FloatEvent : ScriptableEvent<float> { }
    [CreateAssetMenu(menuName = "Events/Integer Event")]
    public class IntEvent : ScriptableEvent<int> { }
    [CreateAssetMenu(menuName = "Events/String Event")]
    public class StringEvent : ScriptableEvent<string> { }

    // Core Implementation
    public class ScriptableEvent<T> : ScriptableObject
    {
        #region Variables

        private UnityEvent<T> _event;

        #endregion


        public virtual void AddListener(UnityAction<T> handler) => _event.AddListener(handler);
        public virtual void RemoveListener(UnityAction<T> handler) => _event.RemoveListener(handler);
        public virtual void Invoke(T args) => _event.Invoke(args);

#if UNITY_EDITOR
        [Button]
        private void Trigger(T arg)
        {
            Invoke(arg);
        }
#endif
    }
}