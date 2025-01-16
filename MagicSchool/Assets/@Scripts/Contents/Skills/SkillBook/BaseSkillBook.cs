using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkillBook : MonoBehaviour // �����Ϸ��� ��ų�� ��ġ�� Ȱ���� ���� �ְ�, ��ų�� ���������� ������ ��쵵 �ֱ⿡ monobehavior�� ���
                                       // >> Hierarchy�� ������ �ʿ䰡 �ִ� ģ������ ������ ����Ƽ���� �����ϴ� �Լ��� ����ϴ� ��찡 ���� ������ ����
                                       // cf. �������� projectile�̳� egoSword ó�� ���� �����ؼ� �ǰ� ������ ������ ���� ���̴�, monobehaiviour�� ��� ���� �ʴ� ���·� �����ص� ��������
{
    // ������ ��ų �Ŵ���
    public List<SkillBase> Skills { get; } = new List<SkillBase>();

    public List<RepeatSkill> RepeatSkills { get; } = new List<RepeatSkill>();
    public List<SequenceSkill> SequenceSkills { get; } = new List<SequenceSkill>();
    public Dictionary<string, SkillBase> BaseSkillDict { get; } = new Dictionary<string, SkillBase>();

    public Define.ObjectType Onwer { get; protected set; }

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

    public T AddSkill<T>(Vector3 position, Transform parent = null) where T : SkillBase // ������ �� �� ���� ��ų�� ������ �ݺ� ���Ǿ�� �ϱ⿡, �Ʒ� �ڵ�� ��ų ȹ��� ������ ���ÿ� ����ǵ��� ����
    {
        System.Type type = typeof(T); // templateID�� Ȯ���ؼ� dataSeet�� ������ �� �� �ȿ� �ִ� ������ ��θ� Ȱ���ϴ� ����� ������, ������ ����� ���� ���÷����� ���
                                      // > ���߿��� Repeat, ������, single�� �� �� if�� üũ�ϰ� if trygetValue �ѹ� �� üũ �

        if (type == typeof(EgoSword)) // Pooling ��ü�� GenerateSpawner()  if not  Managers.Object.Spawn()
        {
            var egoSword = Managers.Object.Spawn<EgoSword>(position, Define.EGO_SWORD_ID);

            Skills.Add(egoSword);
            RepeatSkills.Add(egoSword);
            egoSword.Owner = gameObject.GetComponent<CreatureController>();

            return egoSword as T;
        }
        else if (type == typeof(FireBallSkill))
        {
            FireBallSkill fireBall_Generater = GenerateSpawner<FireBallSkill>(Define.Fire_Ball_ID, parent);

            return fireBall_Generater as T;
        }
        else if (type == typeof(CastingImpact)) // ToDo : spawn�� ���⼭�ϰ� �ݺ��Ǵ� �ڵ�� �ٸ� �Լ��� ���� // ToDo : Lv 0 ó��(���ó��)
        {
            CastingImpact castingImapct_Generater = GenerateSpawner<CastingImpact>(Define.CastingImapct_ID, parent);

            return castingImapct_Generater as T;
        }
        else if (type.IsSubclassOf(typeof(SequenceSkill)))
        {
            var skill = gameObject.GetOrAddComponent<T>();
            Skills.Add(skill);
            SequenceSkills.Add(skill as SequenceSkill);

            skill.Owner = gameObject.GetComponent<CreatureController>();

            return skill as T;
        }

        return null;
    }

    private T GenerateSpawner<T>(string skill_ID, Transform parent = null) where T : SkillBase
    {
        GameObject spawner = new GameObject() { name = skill_ID + Define.Spawner_ID };
        spawner.transform.parent = parent;
        T spawnerT = spawner.GetOrAddComponent<T>();

        Skills.Add(spawnerT);
        BaseSkillDict.Add(skill_ID, spawnerT);
        spawnerT.Owner = gameObject.GetComponent<CreatureController>();

        return spawnerT;
    }

    // �ΰ����ɿ��� �Ǵ��� �ϰ�, ���������� ���� skillbook���� ��ϵ� sequenceskill�� ����� �ϰ� > ��ȹ�� �����̴�

    #region SequenceSkill�� Ʋ���ִ� ����
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
                    index.StopAllCoroutines(); // StopAllCoroutines������� ���� stop�ϴ� �Լ����� ��ų���� ������ �ְ� ���� ȣ���ϴ� �� ���ڴ�
        }
        else if (skillType.IsSubclassOf(typeof(SequenceSkill)))
        {
            foreach (var index in SequenceSkills)
                if (index is T)
                    index.StopAllCoroutines(); // stop�ϴ� �Լ����� ��ų���� ������ �ְ� ���� ȣ���ϴ� �� ���ڴ�
        }
    }

    bool _stopped = false;
    public void StopSkillsAll()
    {
        _stopped = true;

        foreach (var skill in Skills)
            skill.StopAllCoroutines(); // StopAllCoroutines������� ���� stop�ϴ� �Լ����� ��ų���� ������ �ְ� ���� ȣ���ϴ� �� ���ڴ�
    }
    #endregion
}
