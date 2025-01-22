using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerSkillBook : BaseSkillBook
{
    bool _isStartSkill;

    KeyDownEvent _currentkey;
    public KeyDownEvent Inputkey
    {
        get { return _currentkey; }
        set
        {
            if (_isStartSkill == true) // 선 딜레이 중일 시 return
                return;

            pc.CreatureState = CreatureState.Casting;

            _skillKey = _skillKey + $"{value}";
            Debug.Log($"SkillKey -> {_skillKey}");


            switch (value)
            {
                #region N1, N2, N3, N4
                
                case KeyDownEvent.N1:
                    castingImpact.DoSkill();
                    _currentkey = value;
                    break;
                case KeyDownEvent.N2:
                    castingImpact.DoSkill();
                    _currentkey = value;
                    break;
                case KeyDownEvent.N3:
                    castingImpact.DoSkill();
                    _currentkey = value;
                    break;
                case KeyDownEvent.N4:
                    castingImpact.DoSkill();
                    _currentkey = value;
                    break;

                #endregion

                #region Q, W, E, R

                case KeyDownEvent.Q:
                    castingImpact.DoSkill();
                    break;
                case KeyDownEvent.W:
                    castingImpact.DoSkill();
                    break;
                case KeyDownEvent.E:
                    castingImpact.DoSkill();
                    break;
                case KeyDownEvent.R:
                    castingImpact.DoSkill();
                    break;

                #endregion

                #region A, S, D

                case KeyDownEvent.A:
                    castingImpact.InitSize();
                    pc.CreatureState = TryDoSkill();
                    break;
                case KeyDownEvent.S:
                    castingImpact.InitSize();
                    pc.CreatureState = TryDoSkill();
                    break;
                case KeyDownEvent.D:
                    castingImpact.InitSize();
                    pc.CreatureState = TryDoSkill();
                    break;

                #endregion

                default:
                    break;
            }
        }
    }

    SkillBase _skill; // 필요 없을 지도
    string _skillKey;

    CastingImpact castingImpact;
    PlayerController pc;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        pc = GetComponent<PlayerController>();
        castingImpact = AddSkill<CastingImpact>(pc.Indicator.position, pc.SkillBook);

        return true;
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
