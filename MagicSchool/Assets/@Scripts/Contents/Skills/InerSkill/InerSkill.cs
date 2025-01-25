using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InerSkill : SkillBase
{
    public int processCount { get; set; } = 0;

    public InerSkill(string key) : base(Define.ESkillType.None)
    {
        this.Key = key;
    }

    public string Key { get; protected set; }

    public virtual void SetData()
    {
        if (Managers.Data.SkillDic.TryGetValue(Key, out Data.SkillData skillData) == false)
        {
            Debug.LogError("InerSkill LoadData Failed");
            return;
        }

        SkillData = skillData;
    }

    public abstract void DoSkill(Action callBack = null);

    public override void ActivateSkill()
    {
        DoSkill();
    }
}
