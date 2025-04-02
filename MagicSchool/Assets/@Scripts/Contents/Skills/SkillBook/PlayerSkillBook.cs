using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class InputMemorizer
{
    public LinkedList<KeyDownEvent> InputLinkedList { get; private set; } = new LinkedList<KeyDownEvent>(); // �Է� ��� �ý���
    private int _maxSize; // ��� ������ �ִ� ũ��
    private List<int> _usableSkill = new List<int>(); // ��Ī�� ����Ʈ ����

    public InputMemorizer(int size)
    {
        _maxSize = size;
    }

    public void AddInput(KeyDownEvent input)
    {
        // ������ �� remove
        if (InputLinkedList.Count == _maxSize)
            InputLinkedList.RemoveFirst();

        // Add
        InputLinkedList.AddLast(input);
    }

    // List<int>�� �� (���� ����, ���ӵ� �� Ȯ��)
    public bool TrySetUsableSkill(List<int> skillInputValues)
    {
        if (skillInputValues.Count == 0 || InputLinkedList.Count < skillInputValues.Count)
            return false;

        int matchIndex = KMPMatch(InputLinkedList, skillInputValues);
        if (matchIndex != -1)
        {
            _usableSkill = new List<int>(skillInputValues); // ���� ���� �� ����
            return true;
        }
        return false;
    }

    public void RemoveMatchingPattern()
    {
        if (_usableSkill == null || _usableSkill.Count == 0 || InputLinkedList.Count < _usableSkill.Count)
            return;

        int matchIndex = KMPMatch(InputLinkedList, _usableSkill);
        if (matchIndex == -1) return; // ��Ī�� ������ ������ ����

        LinkedListNode<KeyDownEvent> current = InputLinkedList.First;
        for (int i = 0; i < matchIndex; i++)
        {
            current = current.Next; // ���� ���� ��ġ ã��
        }

        // _result�� ũ�⸸ŭ ���ӵ� �� ����
        for (int i = 0; i < _usableSkill.Count && current != null; i++)
        {
            var next = current.Next;
            InputLinkedList.Remove(current);
            current = next;
        }
    }

    // KMP �˰��� ����
    private int KMPMatch(LinkedList<KeyDownEvent> inputLinkedList, List<int> pattern)
    {
        if (pattern.Count == 0 || inputLinkedList.Count < pattern.Count)
            return -1;

        int[] patArray = pattern.ToArray();
        int[] lps = ComputeLPSArray(patArray);

        int i = 0, j = 0;
        KeyDownEvent[] inputArr = new KeyDownEvent[inputLinkedList.Count];
        inputLinkedList.CopyTo(inputArr, 0); // LinkedList �� �迭 ��ȯ (O(N))

        while (i < inputArr.Length)
        {
            if ((int)inputArr[i] == patArray[j])
            {
                i++;
                j++;

                if (j == patArray.Length)
                {
                    Debug.Log($"{i - j} �ڸ������� ������.");
                    return i - j; // ��Ī�� ���� �ε��� ��ȯ
                }
            }
            else
            {
                if (j != 0)
                {
                    j = lps[j - 1]; // LPS �迭�� ������� ����
                }
                else
                {
                    i++; // ��Ī�� �����ϸ� �� ĭ �̵�
                }
            }
        }
        return -1;
    }

    // KMP LPS �迭 ����
    private int[] ComputeLPSArray(int[] pattern)
    {
        int len = 0;
        int[] lps = new int[pattern.Length];
        int i = 1;

        while (i < pattern.Length)
        {
            if (pattern[i] == pattern[len])
            {
                len++;
                lps[i] = len;
                i++;
            }
            else
            {
                if (len != 0) len = lps[len - 1];
                else { lps[i] = 0; i++; }
            }
        }
        return lps;
    }

    // ����� result ��ȯ
    public List<int> GetUsableSkill()
    {
        return _usableSkill;
    }

    public void ResetUsableSkill()
    {
        _usableSkill.Clear();
    }

    public int GetCombinedInputToInt()
    {
        if (_usableSkill.Count == 0) return 0;

        StringBuilder sb = new StringBuilder(InputLinkedList.Count * 3); // ���� ����ȭ

        foreach (var input in _usableSkill)
        {
            sb.Append((int)input); // Enum -> Int ��ȯ �� ���ڿ��� �߰�
        }

        return int.Parse(sb.ToString());
    }

    public string GetCombinedInputToString()
    {
        if (InputLinkedList.Count == 0) return string.Empty;

        StringBuilder sb = new StringBuilder(InputLinkedList.Count * 3); // ���� ����ȭ

        bool first = true; // ù ��° ���̸� " - " ����
        foreach (var input in InputLinkedList)
        {
            if (!first) sb.Append(" - "); // ù ��° ���� �ƴϸ� " - " �߰�
            sb.Append(input);
            first = false;
        }

        return sb.ToString();
    }

    public void Clear()
    {
        InputLinkedList.Clear();
        _usableSkill.Clear();
    }
}

public class PlayerSkillBook : BaseSkillBook
{
    public Action OnSkillValueChanged;

    // public InputTransformer InputTransformer { get; private set; }
    public InputMemorizer InputMemorizer { get; private set; }

    public Dictionary<int, SkillBase> SkillDict { get; } = new Dictionary<int, SkillBase>();

    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // InputTransformer = new InputTransformer();
        InputMemorizer = new InputMemorizer(5);

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

    KeyDownEvent _currentCommand;
    public KeyDownEvent Command
    {
        get { return _currentCommand; }
        set
        {
            // Mp �������� ����, Skill ��� ���� Check
            if (Managers.Game.Player.CheckChangeMp(DefaultSkill.SkillData.UsedMp) == false)
                return;

            if (_owner.CreatureState == CreatureState.DoSkill || _owner.CreatureState == CreatureState.FrontDelay || _owner.CreatureState == CreatureState.BackDelay)
                return;

            _owner.CreatureState = CreatureState.Casting;

            // Add InputValue
            if (value != KeyDownEvent.space)
            {
                InputMemorizer.AddInput(value);

                // CompareWithList -> usableSkill ����
                foreach (SkillBase skillTemp in SkillDict.Values)
                {
                    if (InputMemorizer.TrySetUsableSkill(skillTemp.SkillData.InputValues))
                        Debug.Log($"��� ������ Skill : {skillTemp.SkillData.Name}"); // event ���� UsableSkill ����
                }
            }

            switch (value)
            {
                #region N1, N2, N3, N4
                
                case KeyDownEvent.N1:
                    DefaultSkill.ActivateSkill();
                    _currentCommand = value;
                    break;
                case KeyDownEvent.N2:
                    DefaultSkill.ActivateSkill();
                    _currentCommand = value;
                    break;
                case KeyDownEvent.N3:
                    DefaultSkill.ActivateSkill();
                    _currentCommand = value;
                    break;
                case KeyDownEvent.N4:
                    DefaultSkill.ActivateSkill();
                    _currentCommand = value;
                    break;

                #endregion

                #region Q, W, E, R

                case KeyDownEvent.Q:
                    DefaultSkill.ActivateSkill();
                    break;
                case KeyDownEvent.W:
                    DefaultSkill.ActivateSkill();
                    break;
                case KeyDownEvent.E:
                    DefaultSkill.ActivateSkill();
                    break;
                case KeyDownEvent.R:
                    DefaultSkill.ActivateSkill();
                    break;

                #endregion

                #region A, S, D

                case KeyDownEvent.A:
                    DefaultSkill.ActivateSkill();
                    break;
                case KeyDownEvent.S:
                    DefaultSkill.ActivateSkill();
                    break;
                case KeyDownEvent.D:
                    DefaultSkill.ActivateSkill();
                    break;

                #endregion

                case KeyDownEvent.space:
                    _owner.CreatureState = TryDoSkill();
                    break;

                default:
                    break;
            }

            // Input Value ���� in Skill Navi
            RefreshSkillNavi();
        }
    }

    void RefreshSkillNavi()
    {
        OnSkillValueChanged?.Invoke();
    }

    #region Do Skill

    CreatureState TryDoSkill()
    {
        int inputValue = InputMemorizer.GetCombinedInputToInt();

        if (SkillDict.TryGetValue(inputValue, out SkillBase skill) == false)
        {
            Debug.Log($"Player Do not have --{inputValue}--");

            return CreatureState.Idle;
        }
        else
        {
            skill.ActivateSkillOrDelay();

            _owner.StartWait(skill.SkillData.ActivateSkillDelay + skill.SkillData.SkillDuration);
            Debug.Log($"Do Skill Key : {skill.SkillData.InputValues} in ActivateSkillOrDelay");

            // Refrash Input Value
            InputMemorizer.RemoveMatchingPattern();
            InputMemorizer.ResetUsableSkill();
            RefreshSkillNavi();
        }

        return _owner.CreatureState;
    }

    public void ClearCastingValue()
    {
        
    }

    #endregion
}
