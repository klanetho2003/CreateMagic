using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBreath : AreaSkillBase
{
    public override void SetInfo(CreatureController owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);

        _angleRange = 90; // To Do Data Sheet
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();
    }

    protected override void OnAttackTargetHandler()
    {
        base.OnAttackTargetHandler();
    }

    protected override void OnAttackEvent()
    {
        base.OnAttackEvent();

        ProjectileController _projectile = GenerateProjectile(Owner, Owner.GenerateSkillPosition);
    }
}
