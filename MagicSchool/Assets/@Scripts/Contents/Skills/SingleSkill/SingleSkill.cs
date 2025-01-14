using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleSkill : SkillBase
{
    public SingleSkill(string key) : base(Define.SkillType.Single)
    {
        this.Key = key;
    }

    public string Key { get; protected set; }

    public abstract void DoSkill(Action callBack = null);

    public override void ActivateSkill()
    {
        ActivateSkillDelay(ActivateDelaySecond);
        DoSkill(OnFinishedSingleSkill);
    }

    void OnFinishedSingleSkill()
    {
        // To Do : 후딜레이용 코루틴
        CompleteSkillDelay(CompleteDelaySecond);

        Owner.CreatureState = Define.CreatureState.Idle;
    }
}
