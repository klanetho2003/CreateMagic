using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaImpact : AreaSkillBase
{
    public override void SetInfo(CreatureController owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);

        _angleRange = 90; // To Do Data Sheet

        AddIndicatorComponent();

        if (_indicator != null)
            _indicator.SetInfo(Owner, (Data.MonsterSkillData)SkillData, Define.EIndicatorType.Cone);
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();

        SpawnSpellIndicator();
    }

    protected override void OnAttackTargetHandler()
    {
        base.OnAttackTargetHandler();
    }
}
