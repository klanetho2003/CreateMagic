using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingImpact : SkillBase
{
    // To Do : Data Pasing
    Vector3 _defaultSize = Vector3.one;
    float _moveDistance = 10f;
    float _backSpeed = 30.0f;

    public override void ActivateSkill()
    {
        Vector3 spawnPos;

        if (Owner.CreatureType == Define.ECreatureType.Student)
            spawnPos = Owner.GetComponent<PlayerController>().Shadow.transform.position;
        else
            spawnPos = Owner.CenterPosition;

        GenerateRangeSkill(Owner, spawnPos, AfterTrigger);

        _defaultSize = _defaultSize * 1.2f;
    }

    public void InitSize()
    {
        _defaultSize = Vector3.one;
    }

    public void AfterTrigger(BaseController cc)
    {
        if (cc.IsValid() == false)
            return;
        if (Owner.IsValid() == false)
            return;

        if (cc.GetComponent<CreatureController>().CreatureType != Define.ECreatureType.Monster)
            return;
        if (cc.TryGetComponent(out MonsterController mc))
            return;

        mc.OnDamaged(Owner, this);

        Vector3 dir = cc.transform.position - Owner.transform.position;
        mc.MoveMonsterPosition(dir.normalized, _backSpeed, _moveDistance, () => { mc.CreatureState = Define.CreatureState.Moving; });
    }

    protected override void OnAttackTargetHandler()
    {
        
    }

}
