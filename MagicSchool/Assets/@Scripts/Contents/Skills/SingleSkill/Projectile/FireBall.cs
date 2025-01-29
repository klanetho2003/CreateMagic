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

        Owner.CreatureState = CreatureState.DoSkill;

        if (Owner.CreatureType == ECreatureType.Monster && SkillData.AnimName != null)
            Owner.Anim.Play(SkillData.AnimName);
        else // Player
        {
            string animName = $"{SkillData.AnimName}_LookDown_{Owner.LookDown}";
            Owner.Anim.Play(animName);
        }
    }

    public override void OnSkillDelay(float delaySeconds)
    {
        base.OnSkillDelay(delaySeconds);
    }

    protected virtual void OnAttackEvent()
    {
        GenerateProjectile(Owner, Owner.GenerateSkillPosition, ProjectileOnHit);
    }

    protected override void OnAttackTargetHandler()
    {
        AnimatorStateInfo currentAnim = Owner.Anim.GetCurrentAnimatorStateInfo(0);
        if (currentAnim.IsName(SkillData.AnimName) || currentAnim.IsName($"{SkillData.AnimName}_LookDown_{Owner.LookDown}"))
            OnAttackEvent();
    }

    protected override void OnAnimComplateHandler()
    {
        /*if (Owner.Target.IsValid() == false) // Projectile이라 필요없다
            return;*/

        if (Owner.CreatureState == CreatureState.DoSkill)
            Owner.CreatureState = CreatureState.Idle;
    }

    public void ProjectileOnHit(BaseController cc)
    {
        if (cc.IsValid() == false)
            return;

        cc.OnDamaged(Owner, this);
        //GenerateRangeSkill(Explosion, Owner, _lifeTime, projectile.transform.position, Vector2.one, ExplosionOnHit);
    }



    public void ExplosionOnHit(CreatureController cc)
    {
        if (cc.IsValid() == false)
            return;

        //cc.OnDamaged(Owner, Explosion.damage);
        
        cc.OnBurnEx(Owner, 3,  this); // Extension // To Do : Parsing
    }
}
