using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static Define;

public class LightStaggerSkill : DefaultSkillBase
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
