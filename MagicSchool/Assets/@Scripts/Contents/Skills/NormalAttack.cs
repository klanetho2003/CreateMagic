using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : SkillBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;



        return true;
    }

    public override void SetInfo(CreatureController owner, int monsterSkillTemplateID)
    {
        base.SetInfo(owner, monsterSkillTemplateID);
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();

        Owner.CreatureState = Define.CreatureState.DoSkill;
        Owner.Anim.Play(MonsterSkillData.AnimName);

        Owner.LookAtTarget(Owner.Target);
    }

    protected override void OnAttackTargetHandler()
    {
        if (Owner.Anim.GetCurrentAnimatorStateInfo(0).IsName(MonsterSkillData.AnimName))
            OnAttackEvent();
    }

    protected virtual void OnAttackEvent()
    {
        if (Owner.Target.IsValid() == false)
            return;

        if (MonsterSkillData.ProjectileId == 0) // 근거리 평타
        {
            // Melee
            Owner.Target.OnDamaged(Owner, this);
        }
        else // 원거리 평타
        {
            GenerateProjectile(Owner, Owner.CenterPosition);
        }
    }

    protected override void OnAnimComplateHandler()
    {
        if (Owner.Target.IsValid() == false)
            return;

        if (Owner.CreatureState == Define.CreatureState.DoSkill)
            Owner.CreatureState = Define.CreatureState.Moving;
    }
}
