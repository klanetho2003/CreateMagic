using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CastingImpact : DefaultSkillBase
{
    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        return true;
    }
    #endregion

    public override void SetInfo(CreatureController owner, int monsterSkillTemplateID)
    {
        base.SetInfo(owner, monsterSkillTemplateID);
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
