using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

[Serializable]
public class GameSaveData
{
    //Temp - Value
    public int Wood = 0;
    public int Mineral = 0;
    public int Meat = 0;
    public int Gold = 0;
    // Total Kill Count
    // Clear Stages_List or Last Clear Stage
    // ..

    // Skill
    public List<SkillSaveData> Skills = new List<SkillSaveData>(); // Dictionary ?

    // Item
    public int ItemDbIdGenerator = 1;
    public List<ItemSaveData> Items = new List<ItemSaveData>();

    // Quest
    public List<QuestSaveData> AllQuests = new List<QuestSaveData>();
}

[Serializable]
public class SkillSaveData
{
    public int DataId = 0;
    public int Level = 1;
    public int Exp = 0;
    public SkillOwningState OwningState = SkillOwningState.Lock;

    /* 가령 skill 아니라, 캐릭터를 List로 관리하고 있더라도 평타_SkiillA_SkillB까지 List에 담을 필요 없다.
    Level & 정예화 정보 통해서 Skill을 얻을 수 있기 떄문이다. in RunTime*/
}

public enum SkillOwningState
{
    Lock,       // 발견 X
    Unowned,    // 발견 O, 보유 X
    Owned,      // 보유
    Picked,     // 장비 중
}

[Serializable]
public class ItemSaveData
{
    public int InstanceId;
    public int DbId;
    public int TemplateId;
    public int Count;
    public int EquipSlot; // 장착 + 인벤 + 창고
    // public int OwnerId;
    public int EnchantCount;
}

[Serializable]
public class QuestSaveData
{
    public int TemplateId;
    public EQuestState State = EQuestState.None;
    public List<int> ProgressCount = new List<int>(); // 진행 상황 수치 ex. Monster N Slay
    public DateTime NextResetTime;
}

public class GameManager
{
    public PlayerController Player { get { return Managers.Object?.Player; } }

    // Temp
    List<Transform> _wayPoints = new List<Transform>();
    public List<Transform> WayPoints { get { return _wayPoints; }}
    //

    #region 재화 Temp_Ex
    /*public int Gold { get; set; }

    public Action<int> OnJamCountChanged;

    private int _jam;
    public int Jam
    {
        get { return _jam; }
        set
        {
            _jam = value;
            OnJamCountChanged?.Invoke(value);
        }
    }*/
    #endregion

    #region GameData

    // 진행에 따라 달라지는 Data
    GameSaveData _saveData = new GameSaveData();
    public GameSaveData SaveData { get { return _saveData; } set { _saveData = value; } }

    public int Wood
    {
        get { return _saveData.Wood; }
        private set
        {
            int diff = _saveData.Wood - value;
            _saveData.Wood = value;
            OnBroadcastEvent?.Invoke(EBroadcastEventType.ChangeWood, diff);
        }
    }

    public int Mineral
    {
        get { return _saveData.Mineral; }
        private set
        {
            int diff = _saveData.Mineral - value;
            _saveData.Mineral = value;
            OnBroadcastEvent?.Invoke(EBroadcastEventType.ChangeMineral, diff);
        }
    }

    public int Meat
    {
        get { return _saveData.Meat; }
        private set
        {
            int diff = _saveData.Meat - value;
            _saveData.Meat = value;
            OnBroadcastEvent?.Invoke(EBroadcastEventType.ChangeMeat, diff);
        }
    }

    public int Gold
    {
        get { return _saveData.Gold; }
        private set
        {
            int diff = _saveData.Gold - value;
            _saveData.Gold = value;
            OnBroadcastEvent?.Invoke(EBroadcastEventType.ChangeGold, diff);
        }
    }

    public bool CheckResource(EResourceType eResourceType, int amount)
    {
        switch (eResourceType)
        {
            case EResourceType.Wood:
                return Wood >= amount;
            case EResourceType.Mineral:
                return Mineral >= amount;
            case EResourceType.Meat:
                return Meat >= amount;
            case EResourceType.Gold:
                return Gold >= amount;
            case EResourceType.Dia:
                return true;
            case EResourceType.Materials:
                return true;
            default:
                return false;
        }
    }

    public bool SpendResource(EResourceType eResourceType, int amount)
    {
        if (CheckResource(eResourceType, amount) == false)
            return false;

        switch (eResourceType)
        {
            case EResourceType.Wood:
                Wood -= amount;
                break;
            case EResourceType.Mineral:
                Mineral -= amount;
                break;
            case EResourceType.Meat:
                Meat -= amount;
                break;
            case EResourceType.Gold:
                Gold -= amount;
                break;
            case EResourceType.Dia:
                break;
            case EResourceType.Materials:
                break;
        }

        return true;
    }

    public void EarnResource(EResourceType eResourceType, int amount)
    {
        switch (eResourceType)
        {
            case EResourceType.Wood:
                Wood += amount;
                break;
            case EResourceType.Mineral:
                Mineral += amount;
                break;
            case EResourceType.Meat:
                Meat += amount;
                break;
            case EResourceType.Gold:
                Gold += amount;
                break;
            case EResourceType.Dia:
                break;
            case EResourceType.Materials:
                break;
        }
    }

    public List<SkillSaveData> AllSkills { get { return _saveData.Skills; } }
    public int TotalSkillCount { get { return _saveData.Skills.Count; } }
    public int LockSkillCount { get { return _saveData.Skills.Where(s => s.OwningState == SkillOwningState.Lock).Count(); } }
    public int UnownedSkillCount { get { return _saveData.Skills.Where(s => s.OwningState == SkillOwningState.Unowned).Count(); } }
    public int OwnedSkillCount { get { return _saveData.Skills.Where(s => s.OwningState == SkillOwningState.Owned).Count(); } }
    public int PickedSkillCount { get { return _saveData.Skills.Where(s => s.OwningState == SkillOwningState.Picked).Count(); } }

    public int GenerateItemDbId()
    {
        int itemDbId = _saveData.ItemDbIdGenerator;
        _saveData.ItemDbIdGenerator++;
        return itemDbId;
    }

    #endregion

    #region Save & Load	

    // 왜 파싱 안함? > 생성자 시점에 실행될까봐
    public string Path { get { return Application.persistentDataPath + "/SaveData.json"; } }

    public void InitGame()
    {
        if (File.Exists(Path))
            return;

        // Skill
        // To Do : 패치에 따라 늘어나는 skill을 대처할 수 있도록 수정 - Version 정보를 참조하는 방법?
        var skills = Managers.Data.PlayerSkillDic.Values.ToList();
        foreach (SkillData skill in skills)
        {
            SkillSaveData saveData = new SkillSaveData()
            {
                DataId = skill.DataId,
            };

            SaveData.Skills.Add(saveData);
        }

        // Item
        {
            // Init 지급 Item이 있을 시 MakeItem
        }

        // Quest
        {
            var quests = Managers.Data.QuestDic.Values.ToList();

            foreach (QuestData questData in quests)
            {
                QuestSaveData saveData = new QuestSaveData()
                {
                    TemplateId = questData.DataId,
                    State = EQuestState.None,
                    ProgressCount = new List<int>(),
                    NextResetTime = DateTime.Now,
                };

                for (int i = 0; i < questData.QuestTasks.Count; i++)
                {
                    saveData.ProgressCount.Add(0);
                }

                Managers.Quest.AddQuest(saveData);
            }
        }


        // TEMP - For Debug Skill
        SaveData.Skills[0].OwningState = SkillOwningState.Picked;
        SaveData.Skills[1].OwningState = SkillOwningState.Owned;

        Gold = 100;
        Wood = 100;
        Mineral = 100;
        Meat = 100;
    }

    public void SaveGame()
    {
        // Skill

        // Item
        {
            SaveData.Items.Clear();
            foreach (var item in Managers.Inventory.AllItems)
                SaveData.Items.Add(item.SaveData);
        }

        // Quest
        {
            SaveData.AllQuests.Clear();
            foreach (Quest quest in Managers.Quest.AllQuests.Values)
            {
                SaveData.AllQuests.Add(quest.SaveData);
            }
        }

        string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
        File.WriteAllText(Path, jsonStr);
        Debug.Log($"Save Game Completed : {Path}");
    }

    public bool LoadGame()
    {
        if (File.Exists(Path) == false)
            return false;

        string fileStr = File.ReadAllText(Path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(fileStr);

        if (data != null)
            Managers.Game.SaveData = data;

        // Skill

        // Item
        {
            Managers.Inventory.Clear();

            foreach (ItemSaveData itemSaveData in data.Items)
                Managers.Inventory.AddItem(itemSaveData);
        }

        // Quest
        {
            Managers.Quest.Clear();

            foreach (QuestSaveData questSaveData in data.AllQuests)
            {
                Managers.Quest.AddQuest(questSaveData);
            }

            Managers.Quest.AddUnknownQuests();
        }

        Debug.Log($"Save Game Loaded : {Path}");
        return true;
    }

    #endregion

    #region 이동 & Teleport

    Vector2 _moveDir;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(_moveDir);
        }
    }

    public void TeleportPlayer(Vector3 position)
    {
        TeleportPlayer(Managers.Map.World2Cell(position));
    }

    public void TeleportPlayer(Vector3Int cellPos)
    {
        Vector3Int randCellPos = Managers.Game.GetNearbyPosition(Player, cellPos);

        Player.Cam.enabled = false;
        Managers.Map.MoveTo(Player, randCellPos, forceMove: true);
        Player.Cam.enabled = true;


        // Pet이 있을 경우 아래 코드로 변환
        /*foreach (var hero in Managers.Object.Heroes)
        {
            Vector3Int randCellPos = Managers.Game.GetNearbyPosition(hero, cellPos);
            Managers.Map.MoveTo(hero, randCellPos, forceMove: true);
        }*/

        /*Vector3 worldPos = Managers.Map.Cell2World(cellPos);
        Managers.Object.Camp.ForceMove(worldPos);
        Camera.main.transform.position = worldPos;*/
    }

    #endregion

    #region 전투
    int _killCount;
    public int KillCount
    {
        get { return _killCount; }
        set { _killCount = value;}
    }
    #endregion

    #region Helper
    public Vector3Int GetNearbyPosition(BaseController student, Vector3Int pivot, int range = 5)
    {
        int x = Random.Range(-range, range);
        int y = Random.Range(-range, range);

        for (int i = 0; i < 100; i++)
        {
            Vector3Int randCellPos = pivot + new Vector3Int(x, y, 0);
            if (Managers.Map.CanGo(student, randCellPos))
                return randCellPos;
        }

        Debug.LogError($"GetNearbyPosition Failed");

        return Vector3Int.zero;
    }
    #endregion

    #region Event
    //public event Action<int> OnKillCountChanged;
    public event Action<Vector2> OnMoveDirChanged; //delegate void ExFunc(int a, int b);

    public event Action<EBroadcastEventType, int> OnBroadcastEvent;
    public void BroadcastEvent(EBroadcastEventType eventType, int value)
    {
        OnBroadcastEvent?.Invoke(eventType, value);
    }
    #endregion
}
