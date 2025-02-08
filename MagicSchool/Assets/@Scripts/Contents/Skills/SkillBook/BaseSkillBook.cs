using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class BaseSkillBook : MonoBehaviour // ������ ��ų �Ŵ���
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
    public virtual bool Init() // ���� ������ ���� true�� ��ȯ, �� ���̶� ������ ������ ���� ��� false�� ��ȯ
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }

    #endregion

    public void SetInfo(CreatureController owner, CreatureData creatureData)
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
}
