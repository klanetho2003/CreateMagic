using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillBase : SkillBase
{
    public Data.PlayerSkillData PlayerSkillData { get { return (Data.PlayerSkillData)SkillData; } }

    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;



        return true;
    }
    #endregion

    public override void ActivateSkillOrDelay()
    {
        if (Owner.CheckChangeMp(PlayerSkillData.UsedMp) == false)
            return;

        base.ActivateSkillOrDelay();
    }

    protected override void OnAttackEvent()
    {
        // override
    }
}
