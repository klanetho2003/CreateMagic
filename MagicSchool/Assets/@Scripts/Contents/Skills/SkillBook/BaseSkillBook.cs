using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class BaseSkillBook : MonoBehaviour // 일종의 스킬 매니저
{
    public List<SkillBase> SkillList { get; } = new List<SkillBase>();
    public List<SkillBase> ActivateSkills { get; set; } = new List<SkillBase>();

    public SkillBase DefaultSkill { get; protected set; }
    public SkillBase EnvSkill { get; protected set; }
    public SkillBase ASkill { get; protected set; }
    public SkillBase BSkill { get; protected set; }

    protected CreatureController _owner;

    public SkillBase CurrentSkill
    {
        get
        {
            if (ActivateSkills.Count == 0)
                return DefaultSkill;

            int randomIndex = Random.Range(0, ActivateSkills.Count);
            return ActivateSkills[randomIndex];
        }
    }

    #region Init Method
    
    void Awake()
    {
        Init();
    }

    bool _init = false;
    public virtual bool Init() // 최초 실행일 떄는 true를 반환, 한 번이라도 실행한 내역이 있을 경우 false를 반환
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }

    #endregion

    public /*virtual */void SetInfo(CreatureController owner, CreatureData creatureData)
    {
        _owner = owner;

        AddSkill(creatureData.DefaultSkillId, ESkillSlot.Default);
        AddSkill(creatureData.EnvSkillId, ESkillSlot.Env);
        AddSkill(creatureData.SkillAId, ESkillSlot.A);
        AddSkill(creatureData.SkillBId, ESkillSlot.B);
    }

    public virtual void AddSkill(int skillTemplateID, ESkillSlot skillSlot)
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

    public SkillBase GetReadySkill()
    {
        // Temp
        return SkillList[0];
    }






    public List<SkillBase> Skills { get; } = new List<SkillBase>();

    public List<RepeatSkill> RepeatSkills { get; } = new List<RepeatSkill>();
    public List<SequenceSkill> SequenceSkills { get; } = new List<SequenceSkill>();
    public Dictionary<string, SkillBase> BaseSkillDict { get; } = new Dictionary<string, SkillBase>();

    

    // 인공지능에서 판단을 하건, 순차적으로 여기 skillbook에서 등록된 sequenceskill을 사용을 하건 > 기획의 영역이다

    #region SequenceSkill을 틀어주는 예제
    int _sequenceIndex = 0;

    public void StartNextSequenceSkill()
    {
        if (_stopped)
            return;
        if (SequenceSkills.Count == 0)
            return;

        SequenceSkills[_sequenceIndex].DoSkill(OnFinishedSequenceSkill);
    }

    void OnFinishedSequenceSkill()
    {
        _sequenceIndex = (_sequenceIndex + 1) % SequenceSkills.Count;
        StartNextSequenceSkill();
    }
    #endregion


    #region StopSkill Methods
    public void StopSkill<T>() where T : SkillBase
    {
        Type skillType = typeof(T);

        if (skillType.IsSubclassOf(typeof(RepeatSkill)))
        {
            foreach (var index in RepeatSkills)
                if (index is T)
                    index.StopAllCoroutines(); // StopAllCoroutines사용하지 말고 stop하는 함수들을 스킬마다 가지고 있게 만들어서 호출하는 게 좋겠다
        }
        else if (skillType.IsSubclassOf(typeof(SequenceSkill)))
        {
            foreach (var index in SequenceSkills)
                if (index is T)
                    index.StopAllCoroutines(); // stop하는 함수들을 스킬마다 가지고 있게 만들어서 호출하는 게 좋겠다
        }
    }

    bool _stopped = false;
    public void StopSkillsAll()
    {
        _stopped = true;

        foreach (var skill in Skills)
            skill.StopAllCoroutines(); // StopAllCoroutines사용하지 말고 stop하는 함수들을 스킬마다 가지고 있게 만들어서 호출하는 게 좋겠다
    }
    #endregion
}
