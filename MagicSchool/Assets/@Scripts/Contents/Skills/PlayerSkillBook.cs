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

    //  To Do : Input되었을 때 근접 공격 함수

    public void BuildSKillKey(string inputKey)
    {
        _skillKey = _skillKey + inputKey;

        Debug.Log($"SkillKey -> {_skillKey}");

        if (inputKey != "A" && inputKey != "S" && inputKey != "D")
            return;

        if (SingleSkillDict.TryGetValue(_skillKey, out _skill) == false)
        {
            // To Do : 잘 못 입력하셨습니다. log
            _skillKey = "";
            return;
        }

        // To Do : 선딜 위치 //딜레이는 skills 내부에 delay로 빼기
        _skill.ActivateSkillDelay(0.0f);
        
    }

    public void ActiveSkill()
    {
        _skill.ActivateSkill();

        _skillKey = "";
    }
}
