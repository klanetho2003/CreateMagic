using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class ObjectManager // ID 부여하는 함수, Object들 들고 있는 등
{
    public PlayerController Player { get; private set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
    public HashSet<ProjectileController> ProjectTiles { get; } = new HashSet<ProjectileController>();
    public HashSet<JamController> Jams { get; } = new HashSet<JamController>();

    public T Spawn<T>(Vector3 position, string templateID = null) where T : SkillBase
    {
        System.Type type = typeof(T);

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

        return null;
    }

    public T Spawn<T>(Vector3 position, int templateID = 0) where T : BaseController
    {
        System.Type type = typeof(T);

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
            Data.MonsterData monsterData;

            if (Managers.Data.MonsterDic.TryGetValue(templateID, out monsterData) == false)
            {
                Debug.LogError($"ObjectManager Spawn MOnster Failed {monsterData.name}");
                return null;
            }

            GameObject go = Managers.Resource.Instantiate(monsterData.name, pooling: true);
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

        return null;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        if (obj.IsValid() == false)
        {
            return;
        }

        System.Type type = typeof(T);

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
        else if (typeof(T).IsSubclassOf(typeof(SkillBase)))
        {
            ProjectTiles.Remove(obj as ProjectileController);
            Managers.Resource.Destroy(obj.gameObject);
        }
        else if (type == typeof(BossController))
        {
            Managers.Resource.Destroy(obj.gameObject);
        }
    }

    public void DespawnAllMonsters()
    {
        var monsters = Monsters.ToList();

        foreach (var monster in monsters)
            Managers.Object.Despawn(monster);
    }
}
