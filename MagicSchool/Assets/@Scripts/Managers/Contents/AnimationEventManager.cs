using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AnimationEventManager
{
    // (이벤트 이름, BaseController)와 관련된 콜백 리스트를 저장
    private static readonly Dictionary<BaseController, List<Action>> eventListeners =
        new Dictionary<BaseController, List<Action>>();


    /// 애니메이션 이벤트를 특정 BaseController 인스턴스와 바인딩
    public static void BindEvent(BaseController controller, Action callback) // Action에 조건문 추가 interface를 만들까
    {
        if (controller == null || callback == null)
        {
            Debug.LogError("Invalid event name, controller, or callback.");
            return;
        }

        if (eventListeners.ContainsKey(controller) == false)
        {
            eventListeners[controller] = new List<Action>();
        }

        eventListeners[controller].Add(callback);
    }

    
    /// 특정 BaseController 인스턴스와 바인딩된 애니메이션 이벤트를 해제
    public static void UnbindEvent(BaseController controller, Action callback)
    {
        if (eventListeners.ContainsKey(controller) == false)
        {
            Debug.LogWarning($"No listeners found for event: in controller >> {controller.name}");
            return;
        }

        eventListeners[controller].Remove(callback);

        if (eventListeners[controller].Count == 0)
        {
            eventListeners.Remove(controller);
        }

        Debug.Log($"Unbound event from controller: {controller.name}");
    }

    public static void UnbindEventAll(BaseController controller)
    {
        if (eventListeners.ContainsKey(controller) == false)
            return;

        eventListeners.Remove(controller);
    }

    /// 애니메이션 이벤트 발생 시 특정 BaseController와 연관된 콜백만 호출
    public static void InvokeEvent(BaseController controller, string eventName)
    {
        if (eventListeners.TryGetValue(controller, out var listeners))
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
