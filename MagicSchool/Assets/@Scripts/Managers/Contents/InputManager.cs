using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public event Action<Define.KeyDownEvent> OnKeyDownHandler = null;

    public void OnUpdate()
    {
        if (Input.anyKey == false)
            Managers.Game.MoveDir = Vector2.zero;
        else
        {
            OnKeyInput();
            OnDirInput();
        }
    }
    void OnKeyInput()
    {
        if (OnKeyDownHandler == null)
            return;
        #region 1, 2, 3, 4

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.N1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.N2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.N3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.N4);
        }

        #endregion

        #region Q, W, E, R
        
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.Q);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.W);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.E);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.R);
        }

        #endregion

        #region A, S, D

        else if (Input.GetKeyDown(KeyCode.A))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.A);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.S);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            OnKeyDownHandler.Invoke(Define.KeyDownEvent.D);
        }

        #endregion
    }

    void OnDirInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0)
        {
            return;
        }

        switch (horizontal)
        {
            case 1: //  input "->"
                if (vertical == 0) { Managers.Game.MoveDir = Vector2.right; break; }
                Managers.Game.MoveDir = (vertical == 1) ? (Vector2.up + Vector2.right) : (Vector2.down + Vector2.right);
                break;
            case -1: //  input "<-"
                if (vertical == 0) { Managers.Game.MoveDir = Vector2.left; break; }
                Managers.Game.MoveDir = (vertical == 1) ? (Vector2.up + Vector2.left) : (Vector2.down + Vector2.left);
                break;
            default:
                if (vertical == 1)
                    Managers.Game.MoveDir = Vector2.up;
                else if (vertical == -1)
                    Managers.Game.MoveDir = Vector2.down;
                return;
        }
    }

	public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
	{
		
	}

	public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
	{
        /*click을 통한 이동
        _background.transform.position = eventData.position;
		_handler.transform.position = eventData.position;
		_touchPosition = eventData.position;*/
	}

	public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
	{
        /*click을 통한 이동
        _handler.transform.position = _touchPosition;
		_moveDir = Vector2.zero;

		Managers.Game.MoveDir = _moveDir;*/
	}

	public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
	{
        /*click을 통한 이동
        Vector2 touchDir = (eventData.position - _touchPosition);

		float moveDist = Mathf.Min(touchDir.magnitude, _joystickRadius);
		_moveDir = touchDir.normalized;
		Vector2 newPosition = _touchPosition + _moveDir * moveDist;
		_handler.transform.position = newPosition;

		Managers.Game.MoveDir = _moveDir;*/
	}
}
