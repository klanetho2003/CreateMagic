using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class PlayerSkillBook : BaseSkillBook
{
    public Dictionary<int, SkillBase> SkillDict { get; } = new Dictionary<int, SkillBase>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public override void AddSkill(int skillTemplateID = 0)
    {
        string className = Managers.Data.SkillDic[skillTemplateID].ClassName;

        SkillBase skill = gameObject.AddComponent(Type.GetType(className)) as SkillBase;
        if (skill == null)
            return;

        skill.SetInfo(_owner, skillTemplateID);

        SkillList.Add(skill);
        SkillDict.Add(skillTemplateID, skill);
    }

    
    Queue<int> _inputQueue = new Queue<int>(); // N 번째 값까지만 넣는 방법은 어떤가
    public Queue<int> InputQueue {  get { return _inputQueue; } }

    KeyDownEvent _currentCommand;
    public KeyDownEvent Command
    {
        get { return _currentCommand; }
        set
        {
            if (_owner.CreatureState == CreatureState.DoSkill)
                return;

            _owner.CreatureState = CreatureState.Casting;

            _inputQueue.Enqueue(value.GetHashCode());

            switch (value)
            {
                #region N1, N2, N3, N4
                
                case KeyDownEvent.N1:
                    //castingImpact.DoSkill();
                    _currentCommand = value;
                    break;
                case KeyDownEvent.N2:
                    //castingImpact.DoSkill();
                    _currentCommand = value;
                    break;
                case KeyDownEvent.N3:
                    //castingImpact.DoSkill();
                    _currentCommand = value;
                    break;
                case KeyDownEvent.N4:
                    //castingImpact.DoSkill();
                    _currentCommand = value;
                    break;

                #endregion

                #region Q, W, E, R

                case KeyDownEvent.Q:
                    //castingImpact.DoSkill();
                    break;
                case KeyDownEvent.W:
                    //castingImpact.DoSkill();
                    break;
                case KeyDownEvent.E:
                    //castingImpact.DoSkill();
                    break;
                case KeyDownEvent.R:
                    //castingImpact.DoSkill();
                    break;

                #endregion

                #region A, S, D

                case KeyDownEvent.A:
                    //castingImpact.InitSize();
                    _owner.CreatureState = TryDoSkill();
                    break;
                case KeyDownEvent.S:
                    //castingImpact.InitSize();
                    _owner.CreatureState = TryDoSkill();
                    break;
                case KeyDownEvent.D:
                    //castingImpact.InitSize();
                    _owner.CreatureState = TryDoSkill();
                    break;

                #endregion

                default:
                    break;
            }
        }
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

            Debug.Log($"Do Skill Key : {skillKey}");
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
        Debug.Log($"Do Skill Key : {key}");

        return key;
    }
}
