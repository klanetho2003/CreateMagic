using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingImpact : SingleSkill
{
    public CastingImpact() : base("N1")
    {
        SetData();
    }

    public override void SetData()
    {
        base.SetData();

        Damage = SkillData.damage;

        ActivateDelaySecond = SkillData.activateSkillDelay;
        CompleteDelaySecond = SkillData.completeSkillDelay;
    }

    // To Do : Data Pasing
    Vector3 _defaultSize = Vector3.one;
    float _lifeTime = 0.7f;
    float _moveDistance = 10f;
    float _backSpeed = 30.0f;

    public override void DoSkill(Action callBack = null)
    {
        PlayerController pc = Managers.Game.Player; // Monster가 사용하게 되면 확장 가능하게 바꿔야 한다
        if (pc == null)
            return;

        Vector3 spawnPos = pc.Shadow.transform.position;
        Vector3 dir = Vector2.zero;

        RangeSkillController rc = GenerateRangeSkill(SkillData, Owner, _lifeTime, spawnPos, _defaultSize, AfterTrigger);

        _defaultSize = _defaultSize * 1.2f;
    }

    public void InitSize()
    {
        _defaultSize = Vector3.one;
    }

    public void AfterTrigger(CreatureController cc)
    {
        if (cc.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;

        if (cc.TryGetComponent<MonsterController>(out MonsterController mc))
        {
            mc.OnDamaged(Owner, Damage);
        }
        else
            return;

        Vector3 dir = mc.transform.position - Owner.transform.position;

        mc.MoveMonsterPosition(dir.normalized, _backSpeed, _moveDistance, () => { mc.CreatureState = Define.CreatureState.Moving; });
    }
}
