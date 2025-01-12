using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
    public ObjectType ObjectType { get; protected set; }

    void Awake()
    {
        Init();
    }

    bool _init = false;

    public virtual bool Init() // 최초 실행일 떄는 true를 반환, 한 번이라도 실행한 내역이 있을 경우 false를 반환
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }

    void Update()
    {
        UpdateController();
    }

    public virtual void UpdateController() { }

    void FixedUpdate()
    {
        FixedUpdateController();
    }

    public virtual void FixedUpdateController() { }
}
