using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CastingImpact : AreaSkillBase // Only Player
{
    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        return true;
    }
    #endregion

    protected ProjectileController _projectile;

    public override void SetInfo(CreatureController owner, int monsterSkillTemplateID)
    {
        base.SetInfo(owner, monsterSkillTemplateID);
    }

    public override void ActivateSkill()
    {
        if (Owner.ObjectType != EObjectType.Student)
        {
            Debug.LogError("이 Skill은 오직 Player만 사용할 수 있도록 설계된 Skill 입니다. 개발자에게 문의하세요.");
            return;
        }

        OnAttackEvent();
    }

    protected override void OnAttackEvent()
    {
        Vector3 startSkillPosition = new Vector3(Owner.CenterPosition.x + SkillData.RangeMultipleX, Owner.CenterPosition.y + SkillData.RangeMultipleY);

        // Damage 범위
        float radius = Utils.GetEffectRadius(SkillData.EffectSize);

        // 보여주기용
        _projectile = GenerateProjectile(Owner, startSkillPosition);
        _projectile.transform.localScale *= radius;
        _projectile.Collider.radius = radius;
        
        List<CreatureController> targets = Managers.Object.FindCircleRangeTargets(Owner, startSkillPosition, radius);

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
