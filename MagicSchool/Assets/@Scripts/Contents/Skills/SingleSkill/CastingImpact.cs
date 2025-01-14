using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingImpact : SingleSkill
{
    public CastingImpact() : base("N1QS")
    {
        if (Managers.Data.SkillDic.TryGetValue(Key, out Data.SkillData skillData) == false)
        {
            Debug.LogError("SingleSkill LoadData Failed");
            return;
        }

        SkillData = skillData;

        ActivateDelaySecond = skillData.activateSkillDelay;
        CompleteDelaySecond = skillData.completeSkillDelay;
    }

    public override void DoSkill(Action callBack = null)
    {
        PlayerController pc = Managers.Game.Player;
        if (pc == null)
            return;

        Vector3 spawnPos = pc.transform.position;
        Vector3 dir = Vector2.zero;

        GenerateRangeSkill(SkillData, Owner, spawnPos);

        callBack?.Invoke();
    }
}
