using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ʿ� ���� �� ����
public class FireBall_Explosion : InerSkill
{
    public FireBall_Explosion() : base("N1QAN1")
    {
        SetData();
    }

    public override void SetData()
    {
        base.SetData();

        Damage = SkillData.damage;
    }

    public override void DoSkill(Action callBack = null)
    {
        
    }
}
