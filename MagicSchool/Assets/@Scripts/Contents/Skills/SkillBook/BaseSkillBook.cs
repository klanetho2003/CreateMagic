using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkillBook : MonoBehaviour // 스폰하려는 스킬의 위치를 활용할 수도 있고, 스킬을 프리펩으로 만들어둔 경우도 있기에 monobehavior를 사용
                                       // >> Hierarchy에 등장할 필요가 있는 친구들이 많으니 유니티에서 제공하는 함수를 사용하는 경우가 많을 것으로 예성
                                       // cf. 버프류는 projectile이나 egoSword 처럼 직접 등장해서 피격 판정을 하지는 않을 것이니, monobehaiviour를 상속 받지 않는 형태로 관리해도 괜찮겠지
{
    // 일종의 스킬 매니저
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
    public virtual bool Init() // 최초 실행일 떄는 true를 반환, 한 번이라도 실행한 내역이 있을 경우 false를 반환
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }

    public T AddSkill<T>(Vector3 position, Transform parent = null) where T : SkillBase // 탕탕은 한 번 얻은 스킬은 무조건 반복 사용되어야 하기에, 아래 코드는 스킬 획득과 시전이 동시에 진행되도록 만듦
    {
        System.Type type = typeof(T); // templateID를 확인해서 dataSeet에 접근한 후 그 안에 있는 프리팹 경로를 활용하는 방법이 좋지만, 빠르게 만들기 위해 리플렉션을 사용
                                      // > 나중에는 Repeat, 시퀀스, single로 한 번 if로 체크하고 if trygetValue 한번 더 체크 어때

        if (type == typeof(EgoSword)) // Pooling 객체는 GenerateSpawner()  if not  Managers.Object.Spawn()
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
        else if (type == typeof(CastingImpact)) // ToDo : spawn만 여기서하고 반복되는 코드는 다른 함수로 진행 // ToDo : Lv 0 처리(배움처리)
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
