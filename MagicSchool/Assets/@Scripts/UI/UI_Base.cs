using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
    protected bool _init = false;

    //public abstract void init();

    public virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        return true;

    }

    private void Start()
    {
        Init();
    }

    protected void Bind<T>(Type type) where T : UnityEngine.Object //enum ��ü�� ������ �ƴϴ� �Ѱܹ��� ���ϱ⿡, Reflection�� Ȱ���� �� // C# �����̶� using System �߰� �ʿ�
    {
        string[] names = Enum.GetNames(type); // C#���� �����ϴ� ���
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length]; //null�� Ÿ�԰� �� ������ ä���� �迭 �����
        _objects.Add(typeof(T), objects);                                    // Dictionary�� �迭 �ֱ�

        for (int i = 0; i < names.Length; i++)                               //�迭 ä��� -> �̸�, ������Ʈ�� ������ �ִ��� üũ �� �ֱ�
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Utils.FindChild(this.gameObject, names[i], true);       // ���� (���� ������Ʈ ����)
            else
                objects[i] = Utils.FindChild<T>(this.gameObject, names[i], true);    // ����

            if (objects[i] == null)
                Debug.Log($"Failed to Bind -> {names[i]}");
        }
    }

    public void BindObject(Type type) { Bind<GameObject>(type); }
    public void BindImage(Type type) { Bind<Image>(type); }
    public void BindText(Type type) { Bind<TMP_Text>(type); }
    public void BindButton(Type type) { Bind<Button>(type); }
    public void BindToggle(Type type) { Bind<Toggle>(type); }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    protected GameObject GetGameObject(int idx) { return Get<GameObject>(idx); }
    protected TMP_Text GetText(int idx) { return Get<TMP_Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected Toggle GetToggle(int idx) { return Get<Toggle>(idx); }
    

    public static void BindEvent(GameObject go, Action action = null, Action<BaseEventData> dragAction = null, Define.UIEvent type = Define.UIEvent.Click)
    {
        //evt.OnClickHandler�� ��������Ʈ���� �Ļ��� �̺�Ʈ, action�� ��������Ʈ ����ü(�Լ�)
        //evt.OnClickHandler�� �������� ���� GameObject�� �����ͼ� GetComponents<UI_EventHandler>�� �ϴ� ��

        UI_EventHandler evt = Utils.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Pressed:
                evt.OnPressedHandler -= action;
                evt.OnPressedHandler += action;
                break;
            case Define.UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action;
                evt.OnPointerDownHandler += action;
                break;
            case Define.UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action;
                evt.OnPointerUpHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= dragAction;
                evt.OnDragHandler += dragAction;
                break;
            case Define.UIEvent.BeginDrag:
                evt.OnBeginDragHandler -= dragAction;
                evt.OnBeginDragHandler += dragAction;
                break;
            case Define.UIEvent.EndDrag:
                evt.OnEndDragHandler -= dragAction;
                evt.OnEndDragHandler += dragAction;
                break;
        }
    }
}
