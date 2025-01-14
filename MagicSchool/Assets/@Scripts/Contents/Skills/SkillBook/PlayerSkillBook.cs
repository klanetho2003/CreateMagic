using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillBook : BaseSkillBook
{
    string _skillKey;
    SkillBase _skill;

    PlayerController pc;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        pc = GetComponent<PlayerController>();

        return true;
    }

    bool _isDoSkill;
    public Define.CreatureState BuildSKillKey(string inputKey)
    {
        if (_isDoSkill == true)
            return pc.CreatureState;

        _skillKey = _skillKey + inputKey;
        Debug.Log($"SkillKey -> {_skillKey}");

        if (inputKey != "A" && inputKey != "S" && inputKey != "D")
            return Define.CreatureState.Casting;

        _isDoSkill = BaseSkillDict.TryGetValue(_skillKey, out _skill);
        if (_isDoSkill == false)
        {
            // To Do : 잘 못 입력하셨습니다. log
            Debug.Log($"Player Do not have --{_skillKey}--");

            _skillKey = "";

            return Define.CreatureState.Idle;
        }

        _skill.ActivateSkillDelay(_skill.ActivateDelaySecond, InitKeyInput); //pc.CreatureState = Define.CreatureState.DoSkill;
        return Define.CreatureState.Casting;
    }

    public void ActiveImpact(string inputKey)
    {

    }

    void InitKeyInput()
    {
        _skillKey = "";
        _isDoSkill = false;
    }

    public bool ActiveSkill()
    {
        _skill.ActivateSkill();
        
        return true;
    }
}
