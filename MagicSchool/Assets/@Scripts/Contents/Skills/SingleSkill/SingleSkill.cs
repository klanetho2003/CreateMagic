using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleSkill : SkillBase
{
    public SingleSkill(string key) : base(Define.SkillType.Single)
    {
        this.key = key;
    }

    public string key { get; protected set; }

    public override abstract void ActivateSkill();
}
