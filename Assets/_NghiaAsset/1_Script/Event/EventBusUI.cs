using System;

namespace Turnbase
{
    public static class EventBusUI<T> where T : class
    {
        private static event Action<T> OnEventRaised;

        public static void Subscribe(Action<T> handler)
        {
            OnEventRaised += handler;
        }

        public static void Unsubscribe(Action<T> handler)
        {
            OnEventRaised -= handler;
        }

        public static void Raise(T eventData)
        {
            OnEventRaised?.Invoke(eventData);
        }
    }
}