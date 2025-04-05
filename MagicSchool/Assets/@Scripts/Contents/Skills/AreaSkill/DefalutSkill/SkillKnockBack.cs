using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillKnockBack : AreaSkillBase
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

        _angleRange = 120;
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
        ProjectileController _projectile = GenerateProjectile(Owner, Owner.GenerateSkillPosition);
        // Vector2 lookDir = (Owner.GenerateSkillPosition - Owner.CenterPosition)/*.normalized*/;
        // _projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg);

        // Animation이 나오면 Animation으로 교체 해도됨
        float radius = Utils.GetEffectRadius(SkillData.EffectSize);
        List<CreatureController> targets = Managers.Object.FindConeRangeTargets(Owner, _skillDir, radius, _angleRange);

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
