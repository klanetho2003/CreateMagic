using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.LowLevel;
using static Define;

public class ObjectManager // ID 부여하는 함수, Object들 들고 있는 등
{
    public PlayerController Player { get; private set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
    public HashSet<NpcController> Npcs { get; } = new HashSet<NpcController>();
    public HashSet<ProjectileController> Projectiles { get; } = new HashSet<ProjectileController>();
    public HashSet<EffectBase> Effects { get; } = new HashSet<EffectBase>();
    public HashSet<JamController> Jams { get; } = new HashSet<JamController>();

    public void ShowDamageFont(Vector2 position, float damage, Transform parent, bool isCritical = false)
    {
        GameObject go = Managers.Resource.Instantiate("DamageFont", pooling: true);
        DamageFont damageText = go.GetComponent<DamageFont>();
        damageText.SetInfo(position, damage, parent, isCritical);
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

        if (obj.ObjectType == EObjectType.Student)
        {
            //obj.transform.parent = PlayerRoot;
            PlayerController player = go.GetComponent<PlayerController>();
            Player = player;
            player.SetInfo(templateID);

        }
        else if (obj.ObjectType == EObjectType.Monster)
        {
            //obj.transform.parent = PlayerRoot;
            MonsterController monster = go.GetComponent<MonsterController>();
            Monsters.Add(monster);
            monster.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Npc)
        {
            NpcController npc = go.GetComponent<NpcController>();
            Npcs.Add(npc);
            npc.SetInfo(templateID);
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

        EObjectType objectType = obj.ObjectType;

        if (obj.ObjectType == EObjectType.Student)
        {
            PlayerController player = obj.GetComponent<PlayerController>();
            Player = null;
        }
        else if (obj.ObjectType == EObjectType.Monster)
        {
            MonsterController monster = obj.GetComponent<MonsterController>();
            Monsters.Remove(monster);
        }
        else if (obj.ObjectType == EObjectType.Npc)
        {
            NpcController npc = obj.GetComponent<NpcController>();
            Npcs.Remove(npc);
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

    // Player는 Indicator, Monster는 Center
    public List<CreatureController> FindConeRangeTargets(CreatureController owner, Vector3 dir, float range, int angleRange, bool isAllies = false)
    {
        HashSet<CreatureController> targets = new HashSet<CreatureController>();
        HashSet<CreatureController> ret = new HashSet<CreatureController>();

        EObjectType targetType = Utils.DetermineTargetType(owner.ObjectType, isAllies);

        if (targetType == EObjectType.Monster)
        {
            var objs = Managers.Map.GatherObjects<MonsterController>(owner.GenerateSkillPosition, range, range);
            targets.AddRange(objs);
        }
        else if (targetType == EObjectType.Student)
        {
            var objs = Managers.Map.GatherObjects<PlayerController>(owner.GenerateSkillPosition, range, range);
            targets.AddRange(objs);
        }

        foreach (var target in targets)
        {
            // 1. 거리 안에 있는가?
            var targetPos = target.transform.position;
            float distance = Vector3.Distance(targetPos, owner.GenerateSkillPosition);

            if (distance > range)
                continue;

            // 2. 각도 check
            if (angleRange != 360)
            {
                BaseController ownerTarget = (owner as CreatureController).Target;

                // 2. 부채꼴
                float dot = Vector3.Dot((targetPos - owner.GenerateSkillPosition).normalized, dir.normalized);
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

        EObjectType targetType = Utils.DetermineTargetType(owner.ObjectType, isAllies);

        if (targetType == EObjectType.Monster)
        {
            var objs = Managers.Map.GatherObjects<MonsterController>(startPos/*owner.transform.position*/, range, range); // owner 기준에서 skill 기준으로 변경
            targets.AddRange(objs); 
        }
        else if (targetType == EObjectType.Student)
        {
            var objs = Managers.Map.GatherObjects<PlayerController>(startPos/*owner.transform.position*/, range, range); // owner 기준에서 skill 기준으로 변경
            targets.AddRange(objs);
        }

        foreach (var target in targets)
        {
            // 1. 거리안에 있는지 확인
            var targetPos = target.CenterPosition;
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
