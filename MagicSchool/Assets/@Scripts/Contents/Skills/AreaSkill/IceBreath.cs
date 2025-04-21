using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBreath : PlayerAreaSkillBase
{
    public override void SetInfo(CreatureController owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);

        _angleRange = 120; // To Do Data Sheet
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

        // Set Projectile Dir
        Projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(_skillLookDir.y, _skillLookDir.x) * Mathf.Rad2Deg);
    }
}
