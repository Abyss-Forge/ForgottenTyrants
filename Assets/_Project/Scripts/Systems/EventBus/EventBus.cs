using System.Collections.Generic;
using UnityEngine;

namespace Systems.EventBus
{
    public static class EventBus<T> where T : IBusEvent
    {
        static readonly HashSet<IEventBinding<T>> _bindings = new();

        public static void Register(EventBinding<T> binding) => _bindings.Add(binding);
        public static void Deregister(EventBinding<T> binding) => _bindings.Remove(binding);

        public static void Raise(T @event)
        {
            var snapshot = new HashSet<IEventBinding<T>>(_bindings);

            foreach (var binding in snapshot)
            {
                if (_bindings.Contains(binding))
                {
                    binding.OnEvent.Invoke(@event);
                    binding.OnEventNoArgs.Invoke();
                }
            }
        }

        public static void Clear()
        {
            Debug.Log($"Clearing {typeof(T).Name} bindings");
            _bindings.Clear();
        }

    }
}