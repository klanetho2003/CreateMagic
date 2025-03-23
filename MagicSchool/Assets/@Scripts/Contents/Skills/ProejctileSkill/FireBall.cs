using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class FireBall : SkillBase
{

    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        return true;
    }
    #endregion

    ProjectileController projectile;

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
    }

    protected override void OnAttackEvent()
    {
        projectile = GenerateProjectile(Owner, Owner.GenerateSkillPosition, ProjectileOnHit);
    }

    public void ProjectileOnHit(BaseController cc)
    {
        if (cc.IsValid() == false)
            return;

        cc.OnDamaged(Owner, this);

        Managers.Object.Despawn(projectile);
    }

    protected override void Clear()
    {
        base.Clear();
    }
}
