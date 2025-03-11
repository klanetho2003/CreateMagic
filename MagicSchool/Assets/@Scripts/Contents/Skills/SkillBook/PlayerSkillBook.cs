using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class InputTransformer
{
    private Queue<KeyDownEvent> _inputQueue = new Queue<KeyDownEvent>();
    public List<int> InputList { get; private set; } = new List<int>();

    private const int MAX_INPUTS = 8; //  ASD가 두 자릿수이니, 최총 9자리수가 될 것

    public void AddInput(KeyDownEvent input)
    {
        _inputQueue.Enqueue(input);
        InputList.Add((int)input);
    }

    #region Get Value

    // Dirty Flag 추가를 고민해볼 것
    public int GetCombinedInputToInt()
    {
        if (_inputQueue.Count == 0) return 0;
        if (_inputQueue.Count >= MAX_INPUTS) return 0;

        StringBuilder sb = new StringBuilder(_inputQueue.Count * 2); // 성능 최적화

        foreach (var input in _inputQueue)
        {
            sb.Append((int)input); // Enum -> Int 변환 후 문자열로 추가
        }

        return int.Parse(sb.ToString());
    }

    public string GetCombinedInputToString()
    {
        if (_inputQueue.Count == 0) return string.Empty;

        StringBuilder sb = new StringBuilder(_inputQueue.Count * 3); // 성능 최적화

        bool first = true; // 첫 번째 값이면 " - " 제거
        foreach (var input in _inputQueue)
        {
            if (!first) sb.Append(" - "); // 첫 번째 값이 아니면 " - " 추가
            sb.Append(input);
            first = false;
        }

        return sb.ToString();
    }

    #endregion

    public void Clear()
    {
        _inputQueue.Clear();
        InputList.Clear();
    }
}

public class PlayerSkillBook : BaseSkillBook
{
    public Action<List<SkillBase>> OnSkillValueChanged;

    public InputTransformer InputTransformer { get; private set; }

    public Dictionary<int, SkillBase> SkillDict { get; } = new Dictionary<int, SkillBase>();

    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        InputTransformer = new InputTransformer();

        return true;
    }
    #endregion

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

        // SkillList.Add(skill);
        SkillDict.Add(skillTemplateID, skill);

        switch (skillSlot)
        {
            case ESkillSlot.Default:
                DefaultSkill = skill;
                break;
            case ESkillSlot.Env:
                EnvSkill = skill;
                break;
        }
    }

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

            // Add InputValue
            InputTransformer.AddInput(value);

            // Skill 추천 List 갱신
            foreach (SkillBase skill in SkillDict.Values)
            {
                if (Utils.IsIncludedList(skill.SkillData.InputValues, InputTransformer.InputList))
                    ActivateSkills.Add(skill);
            }
            OnSkillValueChanged.Invoke(ActivateSkills);
            ActivateSkills.Clear();

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

    #region Do Skill

    void DoDefaultSkillAndSetCount()
    {
        DefaultSkill.ActivateSkill();

        DefaultSkill_CastingStack++;
    }

    CreatureState TryDoSkill()
    {
        int skillKey = InputTransformer.GetCombinedInputToInt();

        if (SkillDict.TryGetValue(skillKey, out SkillBase skill) == false)
        {
            Debug.Log($"Player Do not have --{skillKey}--");

            //Clear
            ClearSkillValue();

            return CreatureState.Idle;
        }
        else // _isStartSkill == true
        {
            skill.ActivateSkillOrDelay();

            _owner.StartWait(skill.SkillData.ActivateSkillDelay + skill.SkillData.SkillDuration);
            Debug.Log($"Do Skill Key : {skillKey} in ActivateSkillOrDelay");
        }

        //Clear
        ClearSkillValue();

        return _owner.CreatureState;
    }

    void ClearSkillValue()
    {
        InputTransformer.Clear();
        ActivateSkills.Clear();
    }

    #endregion
}
