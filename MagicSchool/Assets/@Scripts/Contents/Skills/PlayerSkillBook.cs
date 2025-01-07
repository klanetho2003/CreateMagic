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
        CastSkill();
    }

    public void CastSkill()
    {
        if (SingleSkillDict.TryGetValue(_skillKey, out _skill) == false)
        {
            // To Do : �� �Է��ϼ̽��ϴ�. log
            _skillKey = "";
            return;
        }

        pc.CreatureState = Define.CreatureState.DoSkill;

        _skill.ActivateSkill();

        _skillKey = "";
    }
}
