using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillBase : SkillBase
{
    public Data.MonsterSkillData MonsterSkillData { get { return (Data.MonsterSkillData)SkillData; } }

    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;



        return true;
    }
    #endregion

    protected override IEnumerator CoCountdownCooldown()
    {
        RemainCoolTime = MonsterSkillData.CoolTime;
        yield return new WaitForSeconds(MonsterSkillData.CoolTime);
        RemainCoolTime = 0;

        // Ready Skill Add
        if (Owner.Skills != null)
            Owner.Skills.ActivateSkills.Add(this);
    }

    protected override void OnAttackEvent()
    {
        // override
    }
}
