using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBreath : AreaSkillBase
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

        ProjectileController _projectile = GenerateProjectile(Owner, Owner.GenerateSkillPosition);
        Vector2 lookDir = (Owner.GenerateSkillPosition - Owner.CenterPosition)/*.normalized*/;

        _projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg);


        // For Debug
        // float radius = Utils.GetEffectRadius(SkillData.EffectSize);
        // _projectile.Collider.radius = radius;
    }
}
