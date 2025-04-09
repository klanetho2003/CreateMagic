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

    public SkillBase DefaultSkill { get; private set; }
    public SkillBase EnvSkill { get; private set; }
    public SkillBase ASkill { get; private set; }
    public SkillBase BSkill { get; private set; }

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

    public virtual void SetInfo(CreatureController owner, CreatureData creatureData)
    {
        _owner = owner;

        MonsterData monsterData = (MonsterData)creatureData;

        AddSkill(monsterData.DefaultSkillId, ESkillSlot.Default_Monster);
        AddSkill(monsterData.SkillAId, ESkillSlot.A_Monster);
        AddSkill(monsterData.SkillBId, ESkillSlot.B_Monster);
    }

    public virtual void AddSkill(int skillTemplateID, ESkillSlot skillSlot)
    {
        if (skillTemplateID == 0)
            return;

        SkillData data;
        if (_owner.ObjectType == EObjectType.Student)
            data = Managers.Data.StudentSkillDic[skillTemplateID];
        else
            data = Managers.Data.MonsterSkillDic[skillTemplateID];

        SkillBase skill = gameObject.AddComponent(Type.GetType(data.ClassName)) as SkillBase;
        if (skill == null)
            return;

        skill.SetInfo(_owner, skillTemplateID);

        SkillList.Add(skill);

        switch (skillSlot)
        {
            case ESkillSlot.Default_Monster:
                DefaultSkill = skill;
                break;
            case ESkillSlot.A_Monster:
                ASkill = skill;
                ActivateSkills.Add(skill);
                break;
            case ESkillSlot.B_Monster:
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
}
