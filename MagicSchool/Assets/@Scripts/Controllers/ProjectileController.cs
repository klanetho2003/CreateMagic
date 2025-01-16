using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : SkillBase
{
    CreatureController _owner;
    Vector3 _moveDir;
    float _speed = 10.0f;
    float _lifeTime;

    public ProjectileController() : base(Define.SkillType.None) { } // 그저 SKillBase의 정보만을 읽기 위해 SKillBase를 상속 받은 것이기에 type을 Mone으로 넣어줬다

    public override bool Init()
    {
        base.Init();

        return true;
    }

    public void SetInfo(Data.SkillData skillData, CreatureController owner, float lifeTime, Vector3 moveDir)
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

    public void InitValue()
    {
        StartDestory(this, _lifeTime);
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
