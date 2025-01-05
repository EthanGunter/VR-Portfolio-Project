using UnityEngine;

namespace SolarStorm.Design
{
    public interface IEvent { }

    public static class EventBus<T> where T : IEvent
    {
        #region Variables

        public delegate void Handler(T args);
        private static event Handler _event;

        #endregion


        public static void AddListener(Handler handler) => _event += handler;
        public static void RemoveListener(Handler handler) => _event -= handler;
        public static void Raise(T args) => _event.Invoke(args);
    }
}