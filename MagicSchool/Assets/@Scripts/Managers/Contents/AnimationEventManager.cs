using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AnimationEventManager
{
    // (이벤트 이름, BaseController)와 관련된 콜백 리스트를 저장
    private static readonly Dictionary<(BaseController, string), List<Action>> eventListeners =
        new Dictionary<(BaseController, string), List<Action>>();


    /// 애니메이션 이벤트를 특정 BaseController 인스턴스와 바인딩
    public static void BindEvent(BaseController controller, string eventName, Action callback)
    {
        if (string.IsNullOrEmpty(eventName) || controller == null || callback == null)
        {
            Debug.LogError("Invalid event name, controller, or callback.");
            return;
        }

        var key = (controller, eventName);

        if (eventListeners.ContainsKey(key) == false)
        {
            eventListeners[key] = new List<Action>();
        }

        eventListeners[key].Add(callback);
    }

    
    /// 특정 BaseController 인스턴스와 바인딩된 애니메이션 이벤트를 해제
    public static void UnbindEvent(BaseController controller, string eventName, Action callback)
    {
        var key = (controller, eventName);

        if (eventListeners.ContainsKey(key) == false)
        {
            Debug.LogWarning($"No listeners found for event: {eventName} and controller: {controller.name}");
            return;
        }

        eventListeners[key].Remove(callback);

        if (eventListeners[key].Count == 0)
        {
            eventListeners.Remove(key);
        }

        Debug.Log($"Unbound event: {eventName} from controller: {controller.name}");
    }

    /// 애니메이션 이벤트 발생 시 특정 BaseController와 연관된 콜백만 호출
    public static void InvokeEvent(BaseController controller, string eventName)
    {
        var key = (controller, eventName);

        if (eventListeners.TryGetValue(key, out var listeners))
        {
            foreach (var listener in listeners)
            {
                listener?.Invoke();
            }
        }
        else
        {
            Debug.LogWarning($"No listeners registered for event: {eventName} and controller: {controller.name}");
        }
    }

    
    /// Unity 애니메이션 이벤트와 연결될 메서드
    public static void OnAnimationEvent(BaseController controller, string eventName)
    {
        InvokeEvent(controller, eventName);
    }
}
