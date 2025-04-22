using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public struct ObjectSpawnInfo
{
	public ObjectSpawnInfo(string name, int dataId, int x, int y, Vector3 worldPos, EObjectType type)
	{
		Name = name;
		DataId = dataId;
		Vector3Int pos = new Vector3Int(x, y, 0);
		CellPos = pos;
		WorldPos = worldPos;
		ObjectType = type;
	}

	public string Name;
	public int DataId;
	public Vector3Int CellPos;
	public Vector3 WorldPos;
	public EObjectType ObjectType;
}

public class RewardSelector
{
    public static List<ItemData> PickRewardOptions(ItemProbabilityData probData, int count = 3)
    {
        List<ItemData> result = new List<ItemData>();
        HashSet<int> alreadyChosenTemplateIds = new HashSet<int>();

        int maxTry = 100; // 무한루프 방지
        int attempts = 0;

        while (result.Count < count && attempts < maxTry)
        {
            attempts++;
            var reward = GetRandomReward(probData);
            if (reward == null) continue;

            if (alreadyChosenTemplateIds.Contains(reward.DataId))
                continue;

            result.Add(reward);
            alreadyChosenTemplateIds.Add(reward.DataId);
        }

        return result;
    }

    public static ItemData GetRandomReward(ItemProbabilityData probData)
    {
        // 1. 필터링: 뽑을 수 있는 Item이 존재하는 등급만 추출
        List<Information> validInfos = probData.informations
            .Where(info => Managers.Inventory.GetRewardItemsByGrade(info.Grade).Count > 0)
            .ToList();

        if (validInfos.Count == 0)
            return null;

        // 2. 확률 기반 등급 추출
        int total = validInfos.Sum(info => info.Probability);
        int rand = UnityEngine.Random.Range(0, total);
        int cumulative = 0;
        Information selected = null;

        foreach (var info in validInfos)
        {
            cumulative += info.Probability;
            if (rand < cumulative)
            {
                selected = info;
                break;
            }
        }

        if (selected == null)
            return null;

        // 3. 해당 등급의 아이템 중 하나 랜덤 추출
        List<int> candidates = Managers.Inventory.GetRewardItemsByGrade(selected.Grade);
        if (candidates.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, candidates.Count);
        int templateId = candidates[index];

        return Managers.Data.ItemDic.TryGetValue(templateId, out var item) ? item : null;
    }
}

public class Stage : MonoBehaviour
{
    private EMonsterWaveType _currentWave = EMonsterWaveType.Standby;
    public EMonsterWaveType CurrentWave
    {
        get { return _currentWave; }
        set
        {
            // Check Wave Clear
            if (IsWaveClear(_currentWave) == false)
                return;

            // KillCount 초기화
            Managers.Game.KillCount = 0;

            // Try Stage Clear
            if (TryClearStage(value))
                return;

            _currentWave = value;
            StartWave(value);
        }
    }

    [SerializeField]
    private List<BaseController> _spawnObjects = new List<BaseController>();
    private List<ObjectSpawnInfo> _spawnBaseInfos = new List<ObjectSpawnInfo>();
    private Dictionary<EMonsterWaveType, List<ObjectSpawnInfo>> _waveDataInfos = new Dictionary<EMonsterWaveType, List<ObjectSpawnInfo>>();
    private List<EMonsterWaveType> _waveTypes = new List<EMonsterWaveType>();

    private ObjectSpawnInfo _startSpawnInfo;
    public ObjectSpawnInfo StartSpawnInfo
    {
        get { return _startSpawnInfo; }
        set { _startSpawnInfo = value; }
    }

    public ObjectSpawnInfo WaypointSpawnInfo;
    public int StageIndex { get; set; }
    public Tilemap TilemapObject; // 하이어라키에서 추가
    public List<Tilemap> MonsterWaveData = new List<Tilemap>(); // 하이어라키에서 추가
    public Tilemap TilemapTerrain;
    public bool IsActive = false;
    
    private Grid _grid;

    private RewardSelector _rewardSelector;

    public void SetInfo(int stageIdx)
    {
        StageIndex = stageIdx;
        if (TilemapObject == null)
            Debug.LogError("TilemapObject must be assigned in the inspector.", this);
        
        TilemapTerrain = Utils.FindChild<Tilemap>(gameObject, "Tilemap_floor", true);
        SaveSpawnInfos();

        // Init - 2번 Load되는 문제 해결
        IsActive = true;
        UnLoadStage();
    }

    public bool IsPointInStage(Vector3 position)
    {
        Vector3Int pos = TilemapTerrain.layoutGrid.WorldToCell(position);
        TileBase tile = TilemapTerrain.GetTile(pos);

        if (tile == null)
            return false;

        return true;
    }

    public void LoadStage()
    {
        if (IsActive)
            return;

        IsActive = true;
        gameObject.SetActive(true);
        SpawnObjects(_spawnBaseInfos);
    }

    public void UnLoadStage()
    {
        if (IsActive == false)
            return;

        IsActive = false;
        gameObject.SetActive(false);
        DespawnObjects();
    }

    #region Wave Helprs

    public List<EMonsterWaveType> GetWaveTypesAll() // Init할 때가 아니어도 실시간으로 추가된 Wave를 받을 수 있도록 구현
    {
        _waveTypes.Clear();
        _waveTypes.AddRange(_waveDataInfos.Keys);
        return _waveTypes;
    }

    public bool IsWaveClear(EMonsterWaveType currentWave)
    {
        int goalKill = GetMonsterCountInWave(currentWave);
        int currentKill = Managers.Game.KillCount;

        return (currentKill >= goalKill);
    }

    public bool IsCheckStageClear(EMonsterWaveType NextWave)
    {
        return (MonsterWaveData.Count - 1 < (int)NextWave);
    }

    bool TryClearStage(EMonsterWaveType NextWave)
    {
        if (IsCheckStageClear(NextWave) == false)
            return false;

        Debug.Log($"{gameObject.name} Clear !!!");

        DespawnObjects();

        // To Do NPC Spawn

        // Pick Reward Items
        if (Managers.Data.ItemProbabilityDic.TryGetValue(ItemProbability_Data_Sheet_Id, out ItemProbabilityData itemProbabilityData) == false) // 확률 시트 정보 가져오기
            return false;
        List<ItemData> rewards = RewardSelector.PickRewardOptions(itemProbabilityData, 3);

        // Show Reward UI
        UI_ItemSelectPopup ui = Managers.UI.ShowPopupUI<UI_ItemSelectPopup>();
        ui.SetInfo(rewards);

        Managers.Game.Player.PlayerSkills.ClearCastingValue();

        return true;
    }

    public int GetMonsterCountInWave(EMonsterWaveType waveType)
    {
        if (_waveDataInfos.TryGetValue(waveType, out List<ObjectSpawnInfo> waveData) == false)
        {
            Debug.LogWarning($"WaveDataDintionary Has No Value >> {waveType} in {gameObject.name} Stage");
            return 0;
        }

        return _waveDataInfos[waveType].Count;
    }

    public void StartWave(EMonsterWaveType waveType, bool force = false)
    {
        #region For Design -> if force == true
        if (force)
        {
            Managers.Game.KillCount = 0;
            _currentWave = waveType;
        }
        #endregion

        if (_waveDataInfos.TryGetValue(waveType, out List<ObjectSpawnInfo> waveData) == false)
        {
            Debug.LogWarning($"WaveDataDintionary Has No Value >> {waveType} in {gameObject.name} Stage");
            return;
        }

        DespawnObjects();
        SpawnObjects(waveData);
    }

    #endregion

    #region Spawn & Despawn

    private void SpawnObjects(List<ObjectSpawnInfo> infos)
    {
        foreach (ObjectSpawnInfo info in infos)
        {
            Vector3 worldPos = info.WorldPos;
            Vector3Int cellPos = info.CellPos;
            
            if (Managers.Map.CanGo(null, cellPos) == false)
                return;
            
            switch (info.ObjectType)
            {
                case EObjectType.Monster:
                    MonsterController monster = Managers.Object.Spawn<MonsterController>(worldPos, info.DataId);
                    Managers.Map.MoveTo(monster, cellPos, forceMove: true);
                    _spawnObjects.Add(monster);
                    break;
                case EObjectType.Npc:
                    NpcController npc = Managers.Object.Spawn<NpcController>(worldPos, info.DataId);
                    npc.SetCellPos(cellPos, true);
                    _spawnObjects.Add(npc);
                    break;
                /*case EObjectType.Env:
                    Env env = Managers.Object.Spawn<Env>(worldPos, info.DataId);
                    env.SetCellPos(cellPos, true);
                    _spawnObjects.Add(env);
                    break;*/
            }
        }
    }

    private void DespawnObjects()
    {
        foreach (BaseController obj in _spawnObjects)
        {
            Managers.Map.RemoveObject(obj);

            if (obj.IsValid(ignoreSpawning: true) == false)
                continue;

            switch (obj.ObjectType)
            {
                case EObjectType.Monster:
                    Managers.Object.Despawn(obj as MonsterController);
                    break;
                case EObjectType.Npc:
                    // Managers.Object.Despawn(obj as NpcController); // Temp
                    break;
                /*case EObjectType.Env:
                    Managers.Object.Despawn(obj as Env);
                    break;*/
            }
        }

        _spawnObjects.Clear();
    }

    #endregion

    private void SaveSpawnInfos()
    {
        #region BaseObject
        if (TilemapObject != null)
            TilemapObject.gameObject.SetActive(false);

        for (int y = TilemapObject.cellBounds.yMax; y >= TilemapObject.cellBounds.yMin; y--)
        {
            for (int x = TilemapObject.cellBounds.xMin; x <= TilemapObject.cellBounds.xMax; x++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                CustomTile tile = TilemapObject.GetTile(new Vector3Int(x, y, 0)) as CustomTile;

                if (tile == null)
                    continue;

                Vector3 worldPos = Managers.Map.Cell2World(cellPos);
                ObjectSpawnInfo info = new ObjectSpawnInfo(tile.Name, tile.DataId, x, y, worldPos, tile.ObjectType);
                
                if (tile.isStartPos)
                {
                    StartSpawnInfo = info;
                    continue;
                }
                
                Debug.Log($"{tile.name} , {tile.isWayPoint}, {tile.ObjectType}");
                if (tile.isWayPoint)
                {
                    WaypointSpawnInfo = info;
                }

                _spawnBaseInfos.Add(info);
            }
        }
        #endregion

        #region MonsterWaveData
        if (MonsterWaveData.Count > 0)
        {
            for (int i = 0; i < MonsterWaveData.Count; i++)
            {
                Tilemap data = MonsterWaveData[i];
                data.gameObject.SetActive(false);

                List<ObjectSpawnInfo> _monsterWaveInfos = new List<ObjectSpawnInfo>();

                for (int y = data.cellBounds.yMax; y >= data.cellBounds.yMin; y--)
                {
                    for (int x = data.cellBounds.xMin; x <= data.cellBounds.xMax; x++)
                    {
                        Vector3Int cellPos = new Vector3Int(x, y, 0);
                        CustomTile tile = data.GetTile(new Vector3Int(x, y, 0)) as CustomTile;

                        if (tile == null)
                            continue;

                        Vector3 worldPos = Managers.Map.Cell2World(cellPos);
                        ObjectSpawnInfo info = new ObjectSpawnInfo(tile.Name, tile.DataId, x, y, worldPos, tile.ObjectType);

                        _monsterWaveInfos.Add(info);
                    }
                }

                _waveDataInfos.Add((EMonsterWaveType)i, _monsterWaveInfos);
            }
        }
        #endregion
    }

    void Clear()
    {
        #region Wave Data Clear
        for (int i = 0; i < _waveDataInfos.Count; i++)
        {
            EMonsterWaveType waveType = (EMonsterWaveType)i;

            _waveDataInfos[waveType].Clear();
        }

        _waveDataInfos.Clear();

        _waveTypes.Clear();
        #endregion
        DespawnObjects();
        StopAllCoroutines();
    }
}