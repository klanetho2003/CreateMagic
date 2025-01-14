using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSkillController : SkillBase
{
    CreatureController _owner;
    Action<GameObject> _afterTrigger;

    public RangeSkillController() : base(Define.SkillType.None) { } // ���� SKillBase�� �������� �б� ���� SKillBase�� ��� ���� ���̱⿡ type�� Mone���� �־����

    public override bool Init()
    {
        base.Init();



        return true;
    }

    public void SetInfo(Data.SkillData skillData, CreatureController owner, Action<GameObject> afterTrigger = null)
    {
        if (skillData == null)
        {
            Debug.LogError("RangeSkillController SetInfo Failed");
            return;
        }

        _owner = owner;
        _afterTrigger = afterTrigger;
        SkillData = skillData;
        // ToDo : Data Paring
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _afterTrigger.Invoke(collision.gameObject);
    }
}

