using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public abstract class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    #region Init Method

    private void Awake()
    {
        Init();
    }

    protected bool _init = false;
    public virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        return true;

    }
    #endregion

    protected void Bind<T>(Type type) where T : UnityEngine.Object //enum ��ü�� ������ �ƴϴ� �Ѱܹ��� ���ϱ⿡, Reflection�� Ȱ���� �� // C# �����̶� using System �߰� �ʿ�
    {
        string[] names = Enum.GetNames(type); // C#���� �����ϴ� ���
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length]; //null�� Ÿ�԰� �� ������ ä���� �迭 �����
        _objects.Add(typeof(T), objects);                                    // Dictionary�� �迭 �ֱ�

        for (int i = 0; i < names.Length; i++)                               //�迭 ä��� -> �̸�, ������Ʈ�� ������ �ִ��� üũ �� �ֱ�
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Utils.FindChild(gameObject, names[i], true);       // ���� (���� ������Ʈ ����)
            else
                objects[i] = Utils.FindChild<T>(gameObject, names[i], true);    // ����

            if (objects[i] == null)
                Debug.Log($"Failed to Bind -> {names[i]}");
        }
    }

    public void BindObjects(Type type) { Bind<GameObject>(type); }
    public void BindImages(Type type) { Bind<Image>(type); }
    public void BindTexts(Type type) { Bind<TMP_Text>(type); }
    public void BindButtons(Type type) { Bind<Button>(type); }
    public void BindToggles(Type type) { Bind<Toggle>(type); }
    public void BindSliders(Type type) { Bind<Slider>(type); }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected TMP_Text GetText(int idx) { return Get<TMP_Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected Toggle GetToggle(int idx) { return Get<Toggle>(idx); }
    protected Slider GetSliders(int idx) { return Get<Slider>(idx); }
    

    public static void BindEvent(GameObject go, Action<PointerEventData> action = null, UIEvent type = UIEvent.Click)
    {
        
        //evt.OnClickHandler�� ��������Ʈ���� �Ļ��� �̺�Ʈ, action�� ��������Ʈ ����ü(�Լ�)
        //evt.OnClickHandler�� �������� ���� GameObject�� �����ͼ� GetComponents<UI_EventHandler>�� �ϴ� ��

        UI_EventHandler evt = Utils.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action;
                evt.OnPointerDownHandler += action;
                break;
            case UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action;
                evt.OnPointerUpHandler += action;
                break;
            case UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }

    public static void BindEvent(GameObject go, Action<PointerEventData> action1, Action<PointerEventData> action2, UIEvent type1 = UIEvent.Click, UIEvent type2 = UIEvent.None)
    {

        //evt.OnClickHandler�� ��������Ʈ���� �Ļ��� �̺�Ʈ, action�� ��������Ʈ ����ü(�Լ�)
        //evt.OnClickHandler�� �������� ���� GameObject�� �����ͼ� GetComponents<UI_EventHandler>�� �ϴ� ��

        UI_EventHandler evt = Utils.GetOrAddComponent<UI_EventHandler>(go);

        switch (type1)
        {
            case UIEvent.Click:
                evt.OnClickHandler -= action1;
                evt.OnClickHandler += action1;
                break;
            case UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action1;
                evt.OnPointerDownHandler += action1;
                break;
            case UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action1;
                evt.OnPointerUpHandler += action1;
                break;
            case UIEvent.Drag:
                evt.OnDragHandler -= action1;
                evt.OnDragHandler += action1;
                break;
        }

        switch (type2)
        {
            case UIEvent.Click:
                evt.OnClickHandler -= action2;
                evt.OnClickHandler += action2;
                break;
            case UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action2;
                evt.OnPointerDownHandler += action2;
                break;
            case UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action2;
                evt.OnPointerUpHandler += action2;
                break;
            case UIEvent.Drag:
                evt.OnDragHandler -= action2;
                evt.OnDragHandler += action2;
                break;
        }
    }
}
