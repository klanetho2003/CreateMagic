using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AnimationEventManager
{
    // (�̺�Ʈ �̸�, BaseController)�� ���õ� �ݹ� ����Ʈ�� ����
    private static readonly Dictionary<(BaseController, string), List<Action>> eventListeners =
        new Dictionary<(BaseController, string), List<Action>>();


    /// �ִϸ��̼� �̺�Ʈ�� Ư�� BaseController �ν��Ͻ��� ���ε�
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

    
    /// Ư�� BaseController �ν��Ͻ��� ���ε��� �ִϸ��̼� �̺�Ʈ�� ����
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

    /// �ִϸ��̼� �̺�Ʈ �߻� �� Ư�� BaseController�� ������ �ݹ鸸 ȣ��
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

    
    /// Unity �ִϸ��̼� �̺�Ʈ�� ����� �޼���
    public static void OnAnimationEvent(BaseController controller, string eventName)
    {
        InvokeEvent(controller, eventName);
    }
}
