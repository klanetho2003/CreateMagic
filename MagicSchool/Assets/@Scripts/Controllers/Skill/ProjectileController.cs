using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : SkillObjController
{
    Vector3 _moveDir;
    float _speed = 10.0f;

    public override bool Init()
    {
        base.Init();

        return true;
    }

    public void SetInfo(Data.SkillData skillData, CreatureController owner, float lifeTime, Vector3 moveDir, Action<CreatureController> afterTrigger = null)
    {
        if (skillData == null)
        {
            Debug.LogError("ProjectileContoller SetInfo Failed");
            return;
        }

        _owner = owner;
        _lifeTime = lifeTime;
        _moveDir = moveDir;
        SkillData = skillData;
        // ToDo : Data ParingInitValue();

        InitValue();
    }

    public override void InitValue()
    {
        base.InitValue();
    }

    public override void UpdateController()
    {
        base.UpdateController();

        transform.position += _moveDir * _speed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: _moveDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, int.MaxValue);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        MonsterController mc = collision.gameObject.GetComponent<MonsterController>();

        if (mc.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;

        mc.OnDamaged(_owner, SkillData.damage);

        StopDestory(); //코루틴 해제

        Managers.Object.Despawn(this);
    }
}
