using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventTerminal
{
    private static readonly Dictionary<Type, List<Delegate>> _eventListeners = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Reset()
    {
        _eventListeners.Clear();
    }

    public static void RaiseEvent<T>(T eventData)
    {
        Type eventType = typeof(T);

        if (_eventListeners.TryGetValue(eventType, out var listeners))
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                if (listeners[i] is Action<T> typedListener)
                {
                    typedListener.Invoke(eventData);
                }
            }
        }
    }

    public static void AddListener<T>(Action<T> listener)
    {
        if (listener == null) throw new ArgumentNullException(nameof(listener));

        Type eventType = typeof(T);

        if (!_eventListeners.TryGetValue(eventType, out var listeners))
        {
            listeners = new List<Delegate>();
            _eventListeners[eventType] = listeners;
        }

        listeners.Add(listener);
    }

    public static void RemoveListener<T>(Action<T> listener)
    {
        if (listener == null) throw new ArgumentNullException(nameof(listener));

        Type eventType = typeof(T);

        if (_eventListeners.TryGetValue(eventType, out var listeners))
        {
            listeners.Remove(listener);

            if (listeners.Count == 0)
            {
                _eventListeners.Remove(eventType);
            }
        }
    }
}
