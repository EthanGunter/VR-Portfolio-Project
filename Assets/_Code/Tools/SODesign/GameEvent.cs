
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SolarStorm.UnityToolkit
{
    [Serializable]
    [CreateAssetMenu(menuName = "Shared Data/Game Event")]
    public class GameEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListener> eventListeners = new();
        private event Action evt;

        public void Raise()
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised();
            evt?.Invoke();
        }

        public void AddListener(Action listener)
        {
            evt += listener;
        }
        public void RemoveListener(Action listener)
        {
            evt -= listener;
        }

        public void AddListener(GameEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void RemoveListener(GameEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
}
