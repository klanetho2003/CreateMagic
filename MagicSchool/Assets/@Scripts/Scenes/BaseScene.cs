using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public abstract class BaseScene : MonoBehaviour
{
	public EScene SceneType { get; protected set; } = EScene.Unknown;

    #region Init Method
    void Awake()
    {
        Init();
    }

    bool _init = false;

    public virtual bool Init() // 최초 실행일 떄는 true를 반환, 한 번이라도 실행한 내역이 있을 경우 false를 반환
    {
        if (_init)
            return false;

        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
        {
            GameObject go = new GameObject() { name = "@EventSystem" };
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }

        _init = true;
        return true;
    }
    #endregion

    public abstract void Clear();
}
