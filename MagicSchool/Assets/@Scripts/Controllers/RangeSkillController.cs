using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSkillController : SkillBase
{
    CreatureController _owner;

    public RangeSkillController() : base(Define.SkillType.None) { } // ���� SKillBase�� �������� �б� ���� SKillBase�� ��� ���� ���̱⿡ type�� Mone���� �־����

    public override bool Init()
    {
        base.Init();



        return true;
    }

    public void SetInfo(Data.SkillData skillData, CreatureController owner)
    {
        /*if (skillData == null)
        {
            Debug.LogError("ProjectileContoller SetInfo Failed");
            return;
        }

        _owner = owner;
        _moveDir = moveDir;
        SkillData = skillData;*/
        // ToDo : Data Paring
    }

    public override void UpdateController()
    {
        base.UpdateController();
    }
}

