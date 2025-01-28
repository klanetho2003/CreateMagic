using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (SkillData.AnimName != null)
            Owner.Anim.Play(SkillData.AnimName);

        GenerateProjectile(Owner, Owner.GenerateSkillPosition, ProjectileOnHit);
    }

    protected override void OnAnimComplateHandler()
    {
        if (Owner.Target.IsValid() == false)
            return;

        if (Owner.CreatureState == Define.CreatureState.DoSkill)
            Owner.CreatureState = Define.CreatureState.Idle;
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
