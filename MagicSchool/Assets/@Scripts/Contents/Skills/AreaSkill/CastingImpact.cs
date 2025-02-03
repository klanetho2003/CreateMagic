using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CastingImpact : SkillBase
{
    // To Do : Data Pasing
    Vector3 _defaultSize = Vector3.one;
    float _moveDistance = 10f;
    float _backSpeed = 30.0f;

    public override void SetInfo(CreatureController owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);
    }

    public override void ActivateSkill()
    {
        Vector3 spawnPos;

        if (Owner.CreatureType == ECreatureType.Student)
            spawnPos = Owner.GetComponent<PlayerController>().Shadow.transform.position;
        else
            spawnPos = Owner.CenterPosition;

        GenerateProjectile(Owner, spawnPos, AfterTrigger);

        _defaultSize = _defaultSize * 1.2f;
    }

    public void InitSize()
    {
        _defaultSize = Vector3.one;
    }

    public void AfterTrigger(BaseController bc, Vector3 vec)
    {
        if (bc.IsValid() == false)
            return;
        if (Owner.IsValid() == false)
            return;

        if (bc.ObjectType != EObjectType.Creature)
            return;

        CreatureController cc = bc.GetComponent<CreatureController>();

        cc.OnDamaged(Owner, this);

        Vector3 dir = cc.transform.position - Owner.transform.position;
        cc.MoveMonsterPosition(dir.normalized, _backSpeed, _moveDistance, () => { cc.CreatureState = Define.CreatureState.Moving; });
    }

    protected override void OnAttackTargetHandler()
    {
        
    }

}
