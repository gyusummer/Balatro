using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameEventManager : Singleton<IngameEventManager>
{
	private static Dictionary<Type, Delegate> s_eventTable = new Dictionary<Type, Delegate>();

    protected override void Awake()
    {
        base.Awake();
        s_eventTable.Clear();
    }

    /// <summary>
    /// 이벤트 등록
    /// </summary>
    public static void RegisterEvent<T>(Action<T> listener) where T : IngameEventArgs
    {
        var eventType = typeof(T);
        if (s_eventTable.TryGetValue(eventType, out var existingDelegate))
        {
            s_eventTable[eventType] = Delegate.Combine(existingDelegate, listener);
            Debug.Log($"[CardEventManager] Registered event listener for {eventType.Name}");
        }
        else
        {
            s_eventTable[eventType] = listener;
            Debug.Log($"[CardEventManager] Created new event listener for {eventType.Name}");
        }
    }

    /// <summary>
    /// 이벤트 해제
    /// </summary>
    public static void UnregisterEvent<T>(Action<T> listener) where T : IngameEventArgs
    {
        var eventType = typeof(T);
        if (s_eventTable.TryGetValue(eventType, out var existingDelegate))
        {
            var current = Delegate.Remove(existingDelegate, listener);
            if (current == null)
            {
                s_eventTable.Remove(eventType);
                Debug.Log($"[CardEventManager] Unregistered all listeners for {eventType.Name}");
            }
            else
            {
                s_eventTable[eventType] = current;
                Debug.Log($"[CardEventManager] Unregistered listener for {eventType.Name}");
            }
        }
    }

    /// <summary>
    /// 이벤트 호출
    /// </summary>
    public static void CallEvent<T>(T eventArgs) where T : IngameEventArgs
    {
        BroadCastToListeners(eventArgs);
    }

    static void BroadCastToListeners<T>(T eventArgs) where T : IngameEventArgs
    {
        var eventType = eventArgs.GetType();
        Debug.Log($"[CardEventManager] Calling event: {eventType.Name} with args: {eventArgs}");

#if UNITY_EDITOR
        Debug.Log($"[CardEventManager] Dispatching event: {eventType.Name}");
#endif

        if (s_eventTable.TryGetValue(eventType, out var del))
        {
            try
            {
                del.DynamicInvoke(eventArgs);
                Debug.Log($"[CardEventManager] Successfully invoked event: {eventType.Name}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CardEventManager] Error invoking event {eventType.Name}: {ex}");
            }
        }
        else
        {
            //Debug.LogWarning($"[CardEventManager] No listeners registered for {eventType.Name}");
        }
    }
}
