using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class PlayerSkillBook : BaseSkillBook
{
    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
    #endregion

    public Dictionary<int, SkillBase> SkillDict { get; } = new Dictionary<int, SkillBase>();

    public override void AddSkill(int skillTemplateID, ESkillSlot skillSlot)
    {
        if (skillTemplateID == 0)
            return;

        if (Managers.Data.SkillDic.TryGetValue(skillTemplateID, out var data) == false)
        {
            Debug.LogWarning($"AddSkill Failed {skillTemplateID}");
            return;
        }

        SkillBase skill = gameObject.AddComponent(Type.GetType(data.ClassName)) as SkillBase;
        if (skill == null)
            return;


        skill.SetInfo(_owner, skillTemplateID);

        SkillList.Add(skill);
        SkillDict.Add(skillTemplateID, skill);

        switch (skillSlot)
        {
            case ESkillSlot.Default:
                DefaultSkill = skill;
                break;
            case ESkillSlot.Env:
                EnvSkill = skill;
                break;
            case ESkillSlot.A:
                ASkill = skill;
                ActivateSkills.Add(skill);
                break;
            case ESkillSlot.B:
                BSkill = skill;
                ActivateSkills.Add(skill);
                break;
        }
    }
    
    Queue<int> _inputQueue = new Queue<int>(); // N 번째 값까지만 넣는 방법은 어떤가
    public Queue<int> InputQueue {  get { return _inputQueue; } }

    public int DefaultSkill_CastingStack { get; private set; } = 0;

    KeyDownEvent _currentCommand;
    public KeyDownEvent Command
    {
        get { return _currentCommand; }
        set
        {
            if (_owner.CreatureState == CreatureState.DoSkill || _owner.CreatureState == CreatureState.FrontDelay || _owner.CreatureState == CreatureState.BackDelay)
                return;

            _owner.CreatureState = CreatureState.Casting;

            _inputQueue.Enqueue(value.GetHashCode());

            switch (value)
            {
                #region N1, N2, N3, N4
                
                case KeyDownEvent.N1:
                    DoDefaultSkillAndSetCount();
                    _currentCommand = value;
                    break;
                case KeyDownEvent.N2:
                    DoDefaultSkillAndSetCount();
                    _currentCommand = value;
                    break;
                case KeyDownEvent.N3:
                    DoDefaultSkillAndSetCount();
                    _currentCommand = value;
                    break;
                case KeyDownEvent.N4:
                    DoDefaultSkillAndSetCount();
                    _currentCommand = value;
                    break;

                #endregion

                #region Q, W, E, R

                case KeyDownEvent.Q:
                    DoDefaultSkillAndSetCount();
                    break;
                case KeyDownEvent.W:
                    DoDefaultSkillAndSetCount();
                    break;
                case KeyDownEvent.E:
                    DoDefaultSkillAndSetCount();
                    break;
                case KeyDownEvent.R:
                    DoDefaultSkillAndSetCount();
                    break;

                #endregion

                #region A, S, D

                case KeyDownEvent.A:
                    //castingImpact.InitSize();
                    _owner.CreatureState = TryDoSkill();
                    DefaultSkill_CastingStack = 0;
                    break;
                case KeyDownEvent.S:
                    //castingImpact.InitSize();
                    _owner.CreatureState = TryDoSkill();
                    DefaultSkill_CastingStack = 0;
                    break;
                case KeyDownEvent.D:
                    //castingImpact.InitSize();
                    _owner.CreatureState = TryDoSkill();
                    DefaultSkill_CastingStack = 0;
                    break;

                #endregion

                default:
                    break;
            }
        }
    }

    void DoDefaultSkillAndSetCount()
    {
        DefaultSkill.ActivateSkill();

        DefaultSkill_CastingStack++;
    }

    CreatureState TryDoSkill()
    {
        int skillKey = BuildCommandKey();

        if (SkillDict.TryGetValue(skillKey, out SkillBase skill) == false)
        {
            Debug.Log($"Player Do not have --{skillKey}--");

            return CreatureState.Idle;
        }
        else // _isStartSkill == true
        {
            skill.ActivateSkillOrDelay();

            _owner.StartWait(skill.SkillData.ActivateSkillDelay + skill.SkillData.SkillDuration);
            Debug.Log($"Do Skill Key : {skillKey} in ActivateSkillOrDelay");
        }

        return _owner.CreatureState;
    }

    int BuildCommandKey()
    {
        int key = 0;

        while (_inputQueue.Count > 1)
        {
            int index = _inputQueue.Dequeue();

            for (int i = 0; i < _inputQueue.Count; i++)
                index = index * 10;

            key += index;
        }

        key += _inputQueue.Dequeue() - 10;
        Debug.Log($"Do Skill Key : {key} in BuildCommandKey");

        return key;
    }
}
