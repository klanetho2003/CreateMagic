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

    protected PlayerController _pc;
    protected ProjectileController _projectile;

    public override void SetInfo(CreatureController owner, int monsterSkillTemplateID)
    {
        base.SetInfo(owner, monsterSkillTemplateID);

        _pc = Owner as PlayerController;
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
        // Damage 범위
        float radius = Utils.GetEffectRadius(SkillData.EffectSize) * SkillData.ScaleMultiplier;

        // 보여주기용
        _projectile = GenerateProjectile(_pc, _pc.transform.position);
        _projectile.transform.localScale *= radius;
        _projectile.Collider.radius = radius;

        List<CreatureController> targets = Managers.Object.FindCircleRangeTargets(_pc, _pc.transform.position, radius);

        foreach (var target in targets)
        {
            if (target.IsValid())
            {
                target.OnDamaged(_pc, this);
            }
        }
    }

    protected override void Clear()
    {
        base.Clear();
    }
}
