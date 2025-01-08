using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillBook : BaseSkillBook
{
    string _skillKey;
    SingleSkill _skill;

    PlayerController pc;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        pc = GetComponent<PlayerController>();

        return true;
    }

    //  To Do : Input�Ǿ��� �� ���� ���� �Լ�

    public void BuildSKillKey(string inputKey)
    {
        _skillKey = _skillKey + inputKey;

        Debug.Log($"SkillKey -> {_skillKey}");

        if (inputKey != "A" && inputKey != "S" && inputKey != "D")
            return;

        if (SingleSkillDict.TryGetValue(_skillKey, out _skill) == false)
        {
            // To Do : �� �� �Է��ϼ̽��ϴ�. log
            _skillKey = "";
            return;
        }

        // To Do : ���� ��ġ //�����̴� skills ���ο� delay�� ����
        _skill.ActivateSkillDelay(0.0f);
        
    }

    public void ActiveSkill()
    {
        _skill.ActivateSkill();

        _skillKey = "";
    }
}
