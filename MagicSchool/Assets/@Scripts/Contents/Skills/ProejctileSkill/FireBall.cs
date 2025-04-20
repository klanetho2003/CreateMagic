using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class FireBall : PlayerSkillBase
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

    public override void OnSkillDelay(Action action, float delaySeconds)
    {
        base.OnSkillDelay(action, delaySeconds);
    }

    protected override void OnAttackTargetHandler()
    {
        base.OnAttackTargetHandler();
    }

    protected override void OnAttackEvent()
    {
        Vector3 startSkillPosition = new Vector3(Owner.GenerateSkillPosition.x + SkillData.RangeMultipleX, Owner.GenerateSkillPosition.y + SkillData.RangeMultipleY);
        projectile = GenerateProjectile(Owner, startSkillPosition, ProjectileOnHit);
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
