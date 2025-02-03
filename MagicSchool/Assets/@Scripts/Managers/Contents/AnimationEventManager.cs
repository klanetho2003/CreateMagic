using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AnimationEventManager
{
    // (�̺�Ʈ �̸�, BaseController)�� ���õ� �ݹ� ����Ʈ�� ����
    private static readonly Dictionary<BaseController, List<Action>> eventListeners =
        new Dictionary<BaseController, List<Action>>();


    /// �ִϸ��̼� �̺�Ʈ�� Ư�� BaseController �ν��Ͻ��� ���ε�
    public static void BindEvent(BaseController controller, Action callback) // Action�� ���ǹ� �߰� interface�� �����
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

    
    /// Ư�� BaseController �ν��Ͻ��� ���ε��� �ִϸ��̼� �̺�Ʈ�� ����
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

    /// �ִϸ��̼� �̺�Ʈ �߻� �� Ư�� BaseController�� ������ �ݹ鸸 ȣ��
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

    
    /// Unity �ִϸ��̼� �̺�Ʈ�� ����� �޼���
    public static void OnAnimationEvent(BaseController controller, string eventName)
    {
        InvokeEvent(controller, eventName);
    }
}
