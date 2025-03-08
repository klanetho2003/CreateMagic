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

    protected void Bind<T>(Type type) where T : UnityEngine.Object //enum 자체는 변수가 아니니 넘겨받지 못하기에, Reflection을 활용할 것 // C# 문법이라 using System 추가 필요
    {
        string[] names = Enum.GetNames(type); // C#에서 제공하는 기능
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length]; //null로 타입과 들어갈 개수만 채워진 배열 만들기
        _objects.Add(typeof(T), objects);                                    // Dictionary에 배열 넣기

        for (int i = 0; i < names.Length; i++)                               //배열 채우기 -> 이름, 컴포넌트를 가지고 있는지 체크 후 넣기
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Utils.FindChild(gameObject, names[i], true);       // 매핑 (게임 오브젝트 전용)
            else
                objects[i] = Utils.FindChild<T>(gameObject, names[i], true);    // 매핑

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
    

    public static void BindEvent(GameObject go, Action<PointerEventData> action = null, Define.UIEvent type = Define.UIEvent.Click)
    {
        
        //evt.OnClickHandler는 델리게이트에서 파생된 이벤트, action은 델리게이트 그자체(함수)
        //evt.OnClickHandler를 가져오기 위해 GameObject를 가져와서 GetComponents<UI_EventHandler>를 하는 것

        UI_EventHandler evt = Utils.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
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
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }
}
