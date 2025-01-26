using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleSkill : SkillBase
{
    /*public SingleSkill(string key) : base(Define.ESkillType.Single)
    {
        this.Key = key;
    }*/

    public string Key { get; protected set; }

    public virtual void SetData()
    {
        if (Managers.Data.SkillDic.TryGetValue(Key, out Data.SkillData skillData) == false)
        {
            Debug.LogError("SingleSkill LoadData Failed");
            return;
        }

        SkillData = skillData;
    }

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
