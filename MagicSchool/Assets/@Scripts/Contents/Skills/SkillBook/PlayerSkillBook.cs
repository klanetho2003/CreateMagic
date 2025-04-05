using Data;
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
        ResetUsableSkill();
    }
}

public class PlayerSkillBook : BaseSkillBook
{
    public Dictionary<int, SkillBase> SkillDict { get; } = new Dictionary<int, SkillBase>();
    public Dictionary<KeyDownEvent, SkillBase> DefaultSkillDict { get; } = new Dictionary<KeyDownEvent, SkillBase>();

    public Action OnSkillValueChanged;

    public InputMemorizer InputMemorizer { get; private set; }

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

    public override void SetInfo(CreatureController owner, CreatureData creatureData)
    {
        _owner = owner;

        #region Default Skill AddSkill
        StudentData playerData = (StudentData)creatureData;

        AddSkill(playerData.N1_DefaultSkillId, ESkillSlot.N1_Default);
        AddSkill(playerData.N2_DefaultSkillId, ESkillSlot.N2_Default);
        AddSkill(playerData.N3_DefaultSkillId, ESkillSlot.N3_Default);
        AddSkill(playerData.N4_DefaultSkillId, ESkillSlot.N4_Default);

        AddSkill(playerData.Q_DefaultSkillId, ESkillSlot.Q_Default);
        AddSkill(playerData.W_DefaultSkillId, ESkillSlot.W_Default);
        AddSkill(playerData.E_DefaultSkillId, ESkillSlot.E_Default);
        AddSkill(playerData.R_DefaultSkillId, ESkillSlot.R_Default);

        AddSkill(playerData.A_DefaultSkillId, ESkillSlot.A_Default);
        AddSkill(playerData.S_DefaultSkillId, ESkillSlot.S_Default);
        AddSkill(playerData.D_DefaultSkillId, ESkillSlot.D_Default);
        #endregion

        foreach (int skillTemplateId in playerData.TempDevSkillsDataId)
            AddSkill(skillTemplateId, ESkillSlot.CastingSkill);
    }

    public override void AddSkill(int skillTemplateId, ESkillSlot skillSlot)
    {
        if (skillTemplateId == 0)
            return;

        if (Managers.Data.SkillDic.TryGetValue(skillTemplateId, out var data) == false)
        {
            Debug.LogWarning($"AddSkill Failed {skillTemplateId}");
            return;
        }

        SkillBase skill = gameObject.AddComponent(Type.GetType(data.ClassName)) as SkillBase;
        if (skill == null)
            return;

        skill.SetInfo(_owner, skillTemplateId);
        switch (skillSlot)
        {
            #region N1, N2, N3, N4
            case ESkillSlot.N1_Default:
                DefaultSkillDict.Add(KeyDownEvent.N1, skill);
                break;
            case ESkillSlot.N2_Default:
                DefaultSkillDict.Add(KeyDownEvent.N2, skill);
                break;
            case ESkillSlot.N3_Default:
                DefaultSkillDict.Add(KeyDownEvent.N3, skill);
                break;
            case ESkillSlot.N4_Default:
                DefaultSkillDict.Add(KeyDownEvent.N4, skill);
                break;
            #endregion

            #region Q, W, E, R
            case ESkillSlot.Q_Default:
                DefaultSkillDict.Add(KeyDownEvent.Q, skill);
                break;
            case ESkillSlot.W_Default:
                DefaultSkillDict.Add(KeyDownEvent.W, skill);
                break;
            case ESkillSlot.E_Default:
                DefaultSkillDict.Add(KeyDownEvent.E, skill);
                break;
            case ESkillSlot.R_Default:
                DefaultSkillDict.Add(KeyDownEvent.R, skill);
                break;
            #endregion

            #region A, S, D
            case ESkillSlot.A_Default:
                DefaultSkillDict.Add(KeyDownEvent.A, skill);
                break;
            case ESkillSlot.S_Default:
                DefaultSkillDict.Add(KeyDownEvent.S, skill);
                break;
            case ESkillSlot.D_Default:
                DefaultSkillDict.Add(KeyDownEvent.D, skill);
                break;
            #endregion

            default:
                SkillDict.Add(skillTemplateId, skill);
                break;
        }
    }

    KeyDownEvent _currentCommand;
    public KeyDownEvent Command
    {
        get { return _currentCommand; }
        set
        {
            if (_owner.CreatureState == CreatureState.DoSkill || _owner.CreatureState == CreatureState.FrontDelay || _owner.CreatureState == CreatureState.BackDelay)
                return;

            switch (value)
            {
                case KeyDownEvent.N1:
                case KeyDownEvent.N2:
                case KeyDownEvent.N3:
                case KeyDownEvent.N4:
                    TryCasting(value);
                    _currentCommand = value;
                    break;

                case KeyDownEvent.Q:
                case KeyDownEvent.W:
                case KeyDownEvent.E:
                case KeyDownEvent.R:
                case KeyDownEvent.A:
                case KeyDownEvent.S:
                case KeyDownEvent.D:
                    TryCasting(value);
                    break;

                case KeyDownEvent.space:
                    _owner.CreatureState = TryDoSkill();
                    break;
            }

            // Input Value ���� in Skill Navi
            RefreshSkillNavi();
        }
    }

    bool TryCasting(KeyDownEvent inputValue)
    {
        if (DefaultSkillDict.TryGetValue(inputValue, out SkillBase defaultSkill) == false)
            return false;
        if (_owner.CheckChangeMp(defaultSkill.SkillData.UsedMp) == false)
            return false;

        _owner.CreatureState = CreatureState.Casting;

        defaultSkill.ActivateSkill();


        // Add InputValue
        InputMemorizer.AddInput(inputValue);

        // CompareWithList -> usableSkill ����
        foreach (SkillBase skillTemp in SkillDict.Values)
        {
            if (InputMemorizer.TrySetUsableSkill(skillTemp.SkillData.InputValues))
                Debug.Log($"��� ������ Skill : {skillTemp.SkillData.Name}"); // event ���� UsableSkill ����
        }

        return true;
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
        InputMemorizer.Clear();
    }

    #endregion
}
