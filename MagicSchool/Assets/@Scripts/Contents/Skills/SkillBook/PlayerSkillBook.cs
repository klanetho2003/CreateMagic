using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerSkillBook : BaseSkillBook
{
    Define.KeyDownEvent _inputkey;
    Define.KeyDownEvent Inputkey
    {
        get { return _inputkey; }
        set
        {
            if (_isStartSkill == true) // 선 딜레이 중일 시 return
                return;

            _skillKey = _skillKey + $"{value}";
            Debug.Log($"SkillKey -> {_skillKey}");


            switch (value)
            {
                case Define.KeyDownEvent.N1:
                    castingImpact.DoSkill();
                    _inputkey = value;
                    break;
                case Define.KeyDownEvent.N2:
                    castingImpact.DoSkill();
                    _inputkey = value;
                    break;
                case Define.KeyDownEvent.N3:
                    castingImpact.DoSkill();
                    _inputkey = value;
                    break;
                case Define.KeyDownEvent.N4:
                    castingImpact.DoSkill();
                    _inputkey = value;
                    break;

                case Define.KeyDownEvent.Q:
                    break;
                case Define.KeyDownEvent.W:
                    break;
                case Define.KeyDownEvent.E:
                    break;
                case Define.KeyDownEvent.R:
                    break;

                case Define.KeyDownEvent.A:
                    castingImpact.InitSize();
                    pc.CreatureState = TryDoSkill();
                    break;
                case Define.KeyDownEvent.S:
                    castingImpact.InitSize();
                    pc.CreatureState = TryDoSkill();
                    break;
                case Define.KeyDownEvent.D:
                    castingImpact.InitSize();
                    pc.CreatureState = TryDoSkill();
                    break;
                default:
                    break;
            } // CreatrueState 변경 필요
        }
    }

    SkillBase _skill; // 필요 없을 지도
    string _skillKey;

    CastingImpact castingImpact;
    string _currnetImpact; // 프로퍼티를 통해서 변수로 저장하고 있기 때문에
                           // input값을 찾으면 _currnetImpact과 같은 역할을 하는 셍미다

    PlayerController pc;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        pc = GetComponent<PlayerController>();
        castingImpact = AddSkill<CastingImpact>(pc.Indicator.position, pc.SkillBook);

        return true;
    }

    bool _isStartSkill;
    public Define.CreatureState BuildSKillKey(string inputKey)
    {
        if (_isStartSkill == true)
            return pc.CreatureState;

        _skillKey = _skillKey + inputKey;
        Debug.Log($"SkillKey -> {_skillKey}");

        if (inputKey == "A" || inputKey == "S" || inputKey == "D")
        {
            castingImpact.InitSize();

            _isStartSkill = BaseSkillDict.TryGetValue(_skillKey, out _skill);
            if (_isStartSkill == false)
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

    CreatureState TryDoSkill()
    {
        _isStartSkill = BaseSkillDict.TryGetValue(_skillKey, out _skill);
        if (_isStartSkill == false)
        {
            Debug.Log($"Player Do not have --{_skillKey}--");

            _skillKey = "";

            return CreatureState.Idle;
        }
        else // _isStartSkill == true
        {
            _skill.ActivateSkillDelay(_skill.ActivateDelaySecond, InitKeyInput);
        }

        return pc.CreatureState;
    }

    void ActiveImpact(string inputKey) // To Do : 혼합
    {
        if (inputKey == "N1" || inputKey == "N2" || inputKey == "N3")
            _currnetImpact = inputKey;

        castingImpact.DoSkill();
    }

    void InitKeyInput()
    {
        _skillKey = "";
        _isStartSkill = false;
    }

    public bool ActiveSkill()
    {
        _skill.ActivateSkill();
        
        return true;
    }
}
