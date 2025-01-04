using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    /*delegate void Test();     //이벤트와 메소드(함수) 구분해보기
    event Test test = null;*/

    public event Action OnClickHandler = null;
    public event Action OnPressedHandler = null;
    public event Action OnPointerDownHandler = null;
    public event Action OnPointerUpHandler = null;
    public event Action<BaseEventData> OnDragHandler = null;
    public event Action<BaseEventData> OnBeginDragHandler = null;
    public event Action<BaseEventData> OnEndDragHandler = null;

    bool _pressed = false;

    private void Update()
    {
        if (_pressed)
            OnPressedHandler?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
        {
            OnClickHandler.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        OnPointerDownHandler?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = true;
        OnPointerUpHandler?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _pressed = true;
        OnDragHandler?.Invoke(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragHandler?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragHandler?.Invoke(eventData);
    }
}
