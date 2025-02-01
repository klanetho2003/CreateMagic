using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class FireBall : SkillBase
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
    }

    public override void OnSkillDelay(float delaySeconds)
    {
        base.OnSkillDelay(delaySeconds);
    }


    protected override void OnAttackTargetHandler()
    {
        base.OnAttackTargetHandler();

        AnimatorStateInfo currentAnim = Owner.Anim.GetCurrentAnimatorStateInfo(0);
        if (currentAnim.IsName(SkillData.AnimName) || currentAnim.IsName($"{SkillData.AnimName}_LookDown_{Owner.LookDown}"))
            OnAttackEvent();
    }

    protected virtual void OnAttackEvent()
    {
        GenerateProjectile(Owner, Owner.GenerateSkillPosition, ProjectileOnHit);
    }

    public void ProjectileOnHit(BaseController cc, Vector3 position)
    {
        if (cc.IsValid() == false)
            return;

        cc.OnDamaged(Owner, this);
        GenerateRangeSkill(Owner, position, ExplosionOnHit, "FireBall_Explosion.prefab");
    }



    public void ExplosionOnHit(BaseController cc)
    {
        if (cc.IsValid() == false)
            return;

        cc.OnDamaged(Owner, this);
        
        cc.OnBurnEx(Owner, 3,  this); // Extension // To Do : Parsing
    }
}
