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
        DoSkill(OnFinishedSingleSkill);
    }

    void OnFinishedSingleSkill()
    {
        // To Do : �ĵ����̿� �ڷ�ƾ
        CompleteSkillDelay(CompleteDelaySecond); //Idle�� �ĵ����� �Լ����� Change
    }
}
