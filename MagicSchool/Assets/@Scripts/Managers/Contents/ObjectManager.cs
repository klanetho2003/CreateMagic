using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class ObjectManager // ID 부여하는 함수, Object들 들고 있는 등
{
    public PlayerController Player { get; private set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
    public HashSet<ProjectileController> Projectiles { get; } = new HashSet<ProjectileController>();
    public HashSet<JamController> Jams { get; } = new HashSet<JamController>();


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

        #region 구버전
        /*System.Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            // ToDo - Data 연동
            GameObject go = Managers.Resource.Instantiate("Magicion_01.prefab", pooling: true);
            go.name = "player";
            go.transform.position = position;

            PlayerController pc = go.GetOrAddComponent<PlayerController>();
            Player = pc;
            pc.Init();

            return pc as T;
        }
        else if (type == typeof(MonsterController))
        {
            Data.CreatureData creatureData;

            if (Managers.Data.CreatureDic.TryGetValue(templateID, out creatureData) == false)
            {
                Debug.LogError($"ObjectManager Spawn MOnster Failed {creatureData.DescriptionTextID}");
                return null;
            }

            GameObject go = Managers.Resource.Instantiate("creatureData.prefab", pooling: true);
            go.transform.position = position;

            MonsterController mc = go.GetOrAddComponent<MonsterController>();
            Monsters.Add(mc);
            mc.Init();

            return mc as T;
        }
        else if (type == typeof(JamController))
        {
            GameObject go = Managers.Resource.Instantiate(Define.EXP_JAM_PREFAB, pooling: true);
            go.transform.position = position;

            JamController jc = go.GetOrAddComponent<JamController>();
            Jams.Add(jc);
            jc.Init();

            string key = UnityEngine.Random.Range(0, 2) == 0 ? "EXPJam_01.sprite" : "EXPJam_02.sprite";
            Sprite sprite = Managers.Resource.Load<Sprite>(key);
            go.GetComponent<SpriteRenderer>().sprite = sprite;

            //TEMP
            GameObject.Find("@Grid").GetComponent<GridController>().Add(go);

            return jc as T;
        }

        return null;*/
        #endregion
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
