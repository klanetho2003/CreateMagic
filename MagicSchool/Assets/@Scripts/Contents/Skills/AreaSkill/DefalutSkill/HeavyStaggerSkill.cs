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

        // Set Projectile Dir // �������� ������ �� ������� ��
        // Projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(_skillLookDir.y, _skillLookDir.x) * Mathf.Rad2Deg);

        // Damage ���� ���� ����
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
