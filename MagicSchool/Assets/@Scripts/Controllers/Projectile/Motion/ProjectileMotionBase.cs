using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileMotionBase : MonoBehaviour
{
    Coroutine _coLaunchProjectile;

    public Vector3 StartPosition { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    public bool LookAtTarget { get; private set; }
    public Data.ProjectileData ProjectileData { get; private set; }
    public Action EndCallback { get; private set; }

    protected float _speed;

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

        _init = true;
        return true;
    }
    #endregion

    protected void SetInfo(int projectileTemplateID, Vector3 spawnPosition, Vector3 targetPosition, Action endCallBack = null)
    {
        _speed = 5.0f;

        if (projectileTemplateID != 0)
        {
            ProjectileData = Managers.Data.ProjectileDic[projectileTemplateID];
            _speed = ProjectileData.ProjSpeed;
        }
        
        StartPosition = spawnPosition;
        TargetPosition = targetPosition;
        EndCallback = endCallBack;

        LookAtTarget = true; // TEMP

        if (_coLaunchProjectile != null)
            StopCoroutine(_coLaunchProjectile);

        _coLaunchProjectile = StartCoroutine(CoLaunchProjectile());
    }

    protected abstract IEnumerator CoLaunchProjectile(); // 발사 시전 - Update 대용

    protected void LookAt2D(Vector2 forward)
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

    
}
