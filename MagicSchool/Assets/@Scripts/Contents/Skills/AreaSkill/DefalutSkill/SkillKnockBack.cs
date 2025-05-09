using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillKnockBack : DefaultSkillBase
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
        base.ActivateSkill();
    }

    protected override void OnAttackEvent()
    {
        base.OnAttackEvent();
    }

    protected override void Clear()
    {
        base.Clear();
    }
}
