using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillBook : BaseSkillBook
{
    SkillBase _skill;
    string _skillKey;

    CastingImpact castingImpact;
    string _currnetImpact;

    PlayerController pc;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        pc = GetComponent<PlayerController>();
        castingImpact = AddSkill<CastingImpact>(pc.Indicator.position, pc.SkillBook);

        return true;
    }

    bool _isDoSkill;
    public Define.CreatureState BuildSKillKey(string inputKey)
    {
        if (_isDoSkill == true)
            return pc.CreatureState;

        _skillKey = _skillKey + inputKey;
        Debug.Log($"SkillKey -> {_skillKey}");

        if (inputKey == "A" || inputKey == "S" || inputKey == "D")
        {
            castingImpact.InitSize();

            _isDoSkill = BaseSkillDict.TryGetValue(_skillKey, out _skill);
            if (_isDoSkill == false)
            {
                Debug.Log($"Player Do not have --{_skillKey}--");

                _skillKey = "";

                return Define.CreatureState.Idle;
            }

            _skill.ActivateSkillDelay(_skill.ActivateDelaySecond, InitKeyInput); //pc.CreatureState = Define.CreatureState.DoSkill;
        }
        else
        {
            ActiveImpact(inputKey);
            return Define.CreatureState.Casting;
        }    

        return pc.CreatureState;
    }

    void ActiveImpact(string inputKey) // To Do : ШЅЧе
    {
        if (inputKey == "N1" || inputKey == "N2" || inputKey != "N3")
            _currnetImpact = inputKey;

        castingImpact.DoSkill();
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
