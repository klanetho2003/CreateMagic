using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AreaStun : AreaSkillBase
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

        _angleRange = 240;
    }

    public override void ActivateSkill()
    {
        if (Owner.Target != null)
            _skillDir = (Owner.Target.transform.position - Owner.transform.position).normalized;
        else
            _skillDir = (Owner.GenerateSkillPosition - Owner.CenterPosition).normalized;

        OnAttackEvent();
    }

    protected override void OnAttackEvent()
    {
        Vector3 startSkillPosition = new Vector3(Owner.CenterPosition.x + SkillData.RangeMultipleX, Owner.CenterPosition.y + SkillData.RangeMultipleY);

        ProjectileController _projectile = GenerateProjectile(Owner, startSkillPosition);

        // Animation이 나오면 Animation으로 교체 해도됨
        float radius = Utils.GetEffectRadius(SkillData.EffectSize);
        
        List<CreatureController> targets = Managers.Object.FindConeRangeTargets(Owner, startSkillPosition, _skillDir, radius, _angleRange);

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
