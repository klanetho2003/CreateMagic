using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static Define;

public class HeavyStaggerSkill : DefaultSkillBase
{
    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        return true;
    }
    #endregion

    public override void SetInfo(CreatureController owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);

        _angleRange = 90;
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();
    }

    protected override void OnAttackEvent()
    {
        // Projectile
        if (SkillData.ProjectileId != 0)
            Projectile = GenerateProjectile(Owner, _skillcenterPosition);

        // Damage 판정 범위 연산
        float radius = Utils.GetEffectRadius(SkillData.EffectSize);
        List<CreatureController> targets = Managers.Object.FindTriangleRangeTargets(Owner, _skillcenterPosition, _skillLookDir, radius, _angleRange);

        foreach (var target in targets)
        {
            if (target.IsValid())
            {
                target.OnDamaged(Owner, this);
            }
        }
    }

    protected override void Clear()
    {
        base.Clear();
    }
}
