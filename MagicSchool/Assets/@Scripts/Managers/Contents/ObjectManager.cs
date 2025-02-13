using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class ObjectManager // ID 부여하는 함수, Object들 들고 있는 등
{
    public PlayerController Player { get; private set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
    public HashSet<ProjectileController> Projectiles { get; } = new HashSet<ProjectileController>();
    public HashSet<EffectBase> Effects { get; } = new HashSet<EffectBase>();
    public HashSet<JamController> Jams { get; } = new HashSet<JamController>();

    public void ShowDamageFont(Vector2 position, float damage, Transform parent, bool isCritical = false)
    {
        GameObject go = Managers.Resource.Instantiate("DamageFont", pooling: true);
        DamageFont damageText = go.GetComponent<DamageFont>();
        damageText.SetInfo(position, damage, parent, isCritical);
    }

    //Temp
    public T SpawnGameObject<T>(Vector3 position, string prefabName, int templateID = 0) where T : BaseController
    {
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.transform.position = position;

        BaseController obj = go.GetComponent<BaseController>();

        if (obj.ObjectType == EObjectType.ProjecTile)
        {
            ProjectileController projectile = go.GetComponent<ProjectileController>();
            Projectiles.Add(projectile);

            projectile.SetInfo(templateID);
        }

        return obj as T;
    }

    public GameObject SpawnGameObject(Vector3 position, string prefabName)
    {
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.transform.position = position;
        return go;
    }

    public T Spawn<T>(Vector3Int cellPos, int templateID) where T : BaseController
    {
        Vector3 spawnPos = Managers.Map.Cell2World(cellPos);
        return Spawn<T>(spawnPos, templateID);
    }

    public T Spawn<T>(Vector3 position, int templateID = 0) where T : BaseController
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.name = prefabName;
        go.transform.position = position;

        BaseController obj = go.GetComponent<BaseController>();

        if (obj.ObjectType == EObjectType.Creature)
        {
            CreatureController creature = go.GetComponent<CreatureController>();
            switch (creature.CreatureType)
            {
                case ECreatureType.Student:
                    // obj.transform.parent = PlayerRoot;
                    PlayerController player = creature as PlayerController;
                    Player = player;
                    break;
                case ECreatureType.Monster:
                    // obj.transform.parent = MonsterRoot;
                    MonsterController monster = creature as MonsterController;
                    Monsters.Add(monster);
                    break;
            }

            creature.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.ProjecTile)
        {
            ProjectileController projectile = go.GetComponent<ProjectileController>();
            Projectiles.Add(projectile);

            projectile.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Env)
        {
            JamController jc = obj as JamController;
            Jams.Add(jc);

            string key = UnityEngine.Random.Range(0, 2) == 0 ? "EXPJam_01.sprite" : "EXPJam_02.sprite";
            Sprite sprite = Managers.Resource.Load<Sprite>(key);
            obj.SpriteRenderer.sprite = sprite;

            //TEMP
            GameObject.Find("@Grid").GetComponent<GridController>().Add(go);
        }

        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        if (obj.IsValid() == false)
            return;

        if (obj.ObjectType == EObjectType.Creature)
        {
            CreatureController creature = obj.GetComponent<CreatureController>();
            switch (creature.CreatureType)
            {
                case ECreatureType.Student:
                    PlayerController player = creature as PlayerController;
                    Player = null;
                    break;
                case ECreatureType.Monster:
                    MonsterController monster = creature as MonsterController;
                    Monsters.Remove(monster);
                    break;
            }
        }
        else if (obj.ObjectType == EObjectType.ProjecTile)
        {
            ProjectileController projectile = obj as ProjectileController;
            Projectiles.Remove(projectile);
        }
        else if (obj.ObjectType == EObjectType.Env)
        {
            // To Do
        }
        else if (obj.ObjectType == EObjectType.Effect)
        {
            EffectBase effect = obj as EffectBase;
            Effects.Remove(effect);
        }

        Managers.Resource.Destroy(obj.gameObject);

        #region 구버전
        /*System.Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            // ?
        }
        else if (type == typeof(MonsterController))
        {
            Monsters.Remove(obj as MonsterController);
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(JamController))
        {
            Jams.Remove(obj as JamController);
            Managers.Resource.Destroy(obj.gameObject);

            // TEMP
            GameObject.Find("@Grid").GetComponent<GridController>().Remove(obj.gameObject);
        }
        else if (type.IsSubclassOf(typeof(SkillBase)))
        {
            ProjectTiles.Remove(obj as ProjectileController);
            Managers.Resource.Destroy(obj.gameObject);
        }
        if (type == typeof(CastingImpact))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(BossController))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }*/
        #endregion
    }

    public void DespawnAllMonsters()
    {
        var monsters = Monsters.ToList();

        foreach (var monster in monsters)
            Managers.Object.Despawn(monster);
    }

    #region Skill 판정

    public List<CreatureController> FindConeRangeTargets(CreatureController owner, Vector3 dir, float range, int angleRange, bool isAllies = false)
    {
        HashSet<CreatureController> targets = new HashSet<CreatureController>();
        HashSet<CreatureController> ret = new HashSet<CreatureController>();

        ECreatureType targetType = Utils.DetermineTargetType(owner.CreatureType, isAllies);

        if (targetType == ECreatureType.Monster)
        {
            var objs = Managers.Map.GatherObjects<MonsterController>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }
        else if (targetType == ECreatureType.Student)
        {
            var objs = Managers.Map.GatherObjects<PlayerController>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }

        foreach (var target in targets)
        {
            // 1. 거리 안에 있는가?
            var targetPos = target.transform.position;
            float distance = Vector3.Distance(targetPos, owner.transform.position);

            if (distance > range)
                continue;

            // 2. 각도 check
            if (angleRange != 360)
            {
                BaseController ownerTarget = (owner as CreatureController).Target;

                // 2. 부채꼴
                float dot = Vector3.Dot((targetPos - owner.transform.position).normalized, dir.normalized);
                float degree = Mathf.Rad2Deg * Mathf.Acos(dot);

                if (degree > angleRange / 2f)
                    continue;
            }

            ret.Add(target);
        }

        return ret.ToList();
    }

    public List<CreatureController> FindCircleRangeTargets(CreatureController owner, Vector3 startPos, float range, bool isAllies = false)
    {
        HashSet<CreatureController> targets = new HashSet<CreatureController>();
        HashSet<CreatureController> ret = new HashSet<CreatureController>();

        ECreatureType targetType = Utils.DetermineTargetType(owner.CreatureType, isAllies);

        if (targetType == ECreatureType.Monster)
        {
            var objs = Managers.Map.GatherObjects<MonsterController>(owner.transform.position, range, range);
            targets.AddRange(objs); 
        }
        else if (targetType == ECreatureType.Student)
        {
            var objs = Managers.Map.GatherObjects<PlayerController>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }

        foreach (var target in targets)
        {
            // 1. 거리안에 있는지 확인
            var targetPos = target.transform.position;
            float distSqr = (targetPos - startPos).sqrMagnitude;

            if (distSqr < range * range)
                ret.Add(target);
        }

        return ret.ToList();
    }

    #endregion
}



#region PlayerSkillBook의 Key가 int type으로 바뀌면서 필요 없어질 예정

//public T Spawn<T>(Vector3 position, int templateID = 0) where T : SkillBase
//{
//    string prefabName = typeof(T).Name;
//
//    GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
//    go.name = prefabName;
//    go.transform.position = position;
//
//    BaseController obj = null;//go.GetComponent<SkillBase>();
//
//    if (obj.ObjectType == EObjectType.Skill)
//    {
//        if (templateID != 0 && Managers.Data.SkillDic.TryGetValue(templateID, out Data.SkillData data) == false)
//        {
//            Debug.LogError($"ObjectManager Spawn Skill Failed! TryGetValue TemplateID : {templateID}");
//            return null;
//        }
//
//        SkillBase skill = go.GetComponent<SkillBase>();
//        switch (skill.SkillType)
//        {
//            case ESkillType.Repeat:
//                RepeatSkill repeat = skill as RepeatSkill;
//                // 특별 처리
//                break;
//            case ESkillType.Single:
//                SingleSkill single = skill as SingleSkill;
//                // 특별 처리
//                break;
//            case ESkillType.Sequence:
//                SequenceSkill sequence = skill as SequenceSkill;
//                // 특별 처리
//                break;
//        }
//
//        // skill.SetInfo(templateID); // To Do
//    }
//    else if (obj.ObjectType == EObjectType.ProjecTile)
//    {
//        ProjectileController pc = go.GetOrAddComponent<ProjectileController>();
//        Projectiles.Add(pc);
//
//        //pc.SetInfo(templateID); // To Do : setInfo와 SetSkill 로 분기 필요 아니면 매개변수를 다르게 하던가
//    }
//
//    return obj as T;

#region 구버전
/*System.Type type = typeof(T);

if (typeof(T).IsSubclassOf(typeof(SkillBase)))
{
if (Managers.Data.SkillDic.TryGetValue(templateID, out Data.SkillData skillData) == false)
{
    Debug.LogError($"ObjectManager Spawn Skill Failed {templateID}");
    return null;
}

GameObject go = Managers.Resource.Instantiate(skillData.prefab, pooling: true);

go.transform.position = position;
T t = go.GetOrAddComponent<T>();
t.Init();

return t;
}
else if (type == typeof(ProjectileController))
{
GameObject go = Managers.Resource.Instantiate(Managers.Data.SkillDic[templateID].prefab, pooling: true);
go.transform.position = position;

ProjectileController pc = go.GetOrAddComponent<ProjectileController>();
ProjectTiles.Add(pc);
pc.Init();

return pc as T;
}

return null;*/
#endregion

//}

#endregion
