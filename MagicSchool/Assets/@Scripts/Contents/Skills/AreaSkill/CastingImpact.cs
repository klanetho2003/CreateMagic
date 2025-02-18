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

    PlayerController _pc;

    public override void SetInfo(CreatureController owner, int monsterSkillTemplateID)
    {
        base.SetInfo(owner, monsterSkillTemplateID);

        _pc = Owner as PlayerController;
    }

    public override void ActivateSkill()
    {
        if (Owner.CreatureType != ECreatureType.Student)
        {
            Debug.LogError("�� Skill�� ���� Player�� ����� �� �ֵ��� ����� Skill �Դϴ�. �����ڿ��� �����ϼ���.");
            return;
        }

        OnAttackEvent();
    }

    protected override void OnAttackEvent()
    {
        // Damage ����
        float radius = Utils.GetEffectRadius(SkillData.EffectSize) * _pc.PlayerSkills.ScaleMultiple_DefaultSkill;

        // �����ֱ��
        ProjectileController _projectile = GenerateProjectile(_pc, _pc.transform.position);
        _projectile.transform.localScale *= radius;
        _projectile.Collider.radius = radius;

        List<CreatureController> targets = Managers.Object.FindConeRangeTargets(_pc, _skillDir, radius, _angleRange);

        foreach (var target in targets)
        {
            if (target.IsValid())
            {
                target.OnDamaged(_pc, this);
            }
        }
    }
}
