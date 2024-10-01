
using UnityEngine;
using UnityEngine.Events;

namespace SolarStorm.UnityToolkit
{
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public GameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent Response;

        private void OnEnable()
        {
            Event.AddListener(this);
        }

        private void OnDisable()
        {
            Event.RemoveListener(this);
        }

        public void OnEventRaised()
        {
            Response?.Invoke();
        }
    }
}