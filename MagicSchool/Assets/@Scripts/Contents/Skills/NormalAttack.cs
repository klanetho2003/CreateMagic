using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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

        if (SkillData.AnimName != null)
            Owner.Anim.Play(SkillData.AnimName);

        Owner.LookAtTarget(Owner.Target);
    }

    protected override void OnAttackTargetHandler()
    {
        if (Owner.Anim.GetCurrentAnimatorStateInfo(0).IsName(SkillData.AnimName))
            OnAttackEvent();
    }

    protected virtual void OnAttackEvent()
    {
        if (Owner.Target.IsValid() == false)
            return;

        if (SkillData.ProjectileId == 0) // �ٰŸ� ��Ÿ
        {
            // Melee
            Owner.Target.OnDamaged(Owner, this);
        }
        else // ���Ÿ� ��Ÿ
        {
            GenerateProjectile(Owner, Owner.CenterPosition, ProjectileOnHit);
        }
    }

    protected override void OnAnimComplateHandler()
    {
        if (Owner.Target.IsValid() == false)
            return;

        if (Owner.CreatureState == Define.CreatureState.DoSkill)
            Owner.CreatureState = Define.CreatureState.Moving;
    }

    public void ProjectileOnHit(BaseController cc)
    {
        if (cc.IsValid() == false)
            return;

        cc.OnDamaged(Owner, this);
    }
}
