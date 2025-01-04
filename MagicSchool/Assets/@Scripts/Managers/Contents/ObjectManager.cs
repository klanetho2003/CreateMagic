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
            //string name = (templateID == 0) ? "Goblin_01" : "Snake_01";
            string name = ""; // 귀찮으니 하드코딩으로 > To Do : Data시트 연동
            switch (templateID)
            {
                case (int)MonsterID.Goblin:
                    name = "Goblin_01";
                    break;
                case (int)MonsterID.Snake:
                    name = "Snake_01";
                    break;
                case (int)MonsterID.Boss:
                    name = "Boss_01";
                    break;
            }

            GameObject go = Managers.Resource.Instantiate(name + ".prefab", pooling: true);
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
        else if (type == typeof(ProjectileController))
        {
            //ToDo : templateID를 통해서 Prefab의 ID를 불러와야 한다. >> Managers.Data.SkillDic[templateID].prefab
            GameObject go = Managers.Resource.Instantiate(Managers.Data.SkillDic[templateID].prefab, pooling: true);
            go.transform.position = position;

            ProjectileController pc = go.GetOrAddComponent<ProjectileController>();
            ProjectTiles.Add(pc);
            pc.Init();

            return pc as T;
        }
        else if (typeof(T).IsSubclassOf(typeof(SkillBase)))
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
