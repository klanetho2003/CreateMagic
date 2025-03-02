using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

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

public class Stage : MonoBehaviour
{
    private EMonsterWaveType _currentWave = EMonsterWaveType.Standby;
    public EMonsterWaveType CurrentWave
    {
        get { return _currentWave; }
        set
        {
            // Wave 총 개수 Check
            if (MonsterWaveData.Count - 1 < (int)value)
            {
                Debug.Log($"{gameObject.name} Clear !!!");

                // To Do NPC Spawn

                Managers.UI.ShowPopupUI<UI_SkillSelectPopup>();

                return;
            }

            _currentWave = value;
            StartWave(value);
        }
    }

    [SerializeField]
    private List<BaseController> _spawnObjects = new List<BaseController>();
    private List<ObjectSpawnInfo> _spawnInfos = new List<ObjectSpawnInfo>();
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

    public void SetInfo(int stageIdx)
    {
        StageIndex = stageIdx;
        if (TilemapObject == null)
            Debug.LogError("TilemapObject must be assigned in the inspector.", this);
        
        TilemapTerrain = Utils.FindChild<Tilemap>(gameObject, "Tilemap_floor", true);
        SaveSpawnInfos();
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
        SpawnBaseObjects();
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

    public List<EMonsterWaveType> GetWaveTypes() // Init할 때가 아니어도 실시간으로 추가된 Wave를 받을 수 있도록 구현
    {
        _waveTypes.Clear();
        _waveTypes.AddRange(_waveDataInfos.Keys);
        return _waveTypes;
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

    public void StartWave(EMonsterWaveType waveType)
    {
        if (_waveDataInfos.TryGetValue(waveType, out List<ObjectSpawnInfo> currentWave) == false)
            return;

        // KillCount 초기화
        Managers.Game.KillCount = 0;

        foreach (ObjectSpawnInfo info in currentWave)
        {
            Vector3 worldPos = info.WorldPos;
            Vector3Int cellPos = info.CellPos;

            if (Managers.Map.CanGo(null, cellPos) == false)
                return;

            switch (info.ObjectType)
            {
                case EObjectType.Monster:
                    StartSpawnDelay(worldPos, cellPos, info); // Temp > Base로 깔아둬야하는 Object들이기에 딜레이가 없어도 될 듯
                    break;
                    /*case EObjectType.Npc:
                        NpcController npc = Managers.Object.Spawn<NpcController>(worldPos, info.DataId);
                        npc.SetCellPos(cellPos, true);
                        _spawnObjects.Add(npc);
                        break;*/
                    /*case EObjectType.Env:
                        Env env = Managers.Object.Spawn<Env>(worldPos, info.DataId);
                        env.SetCellPos(cellPos, true);
                        _spawnObjects.Add(env);
                        break;*/
            }
        }
    }

    #endregion

    #region Spawn & Despawn

    private void StartSpawnDelay(Vector3 worldPos, Vector3Int cellPos, ObjectSpawnInfo info)
    {
        StartCoroutine(CoSpawnDelay(3, worldPos, cellPos, info));
    }

    IEnumerator CoSpawnDelay(float seconds, Vector3 worldPos, Vector3Int cellPos, ObjectSpawnInfo info)
    {
        GameObject go = Managers.Resource.Instantiate("PositionMaker_temp", pooling: true);
        go.transform.position = worldPos;

        yield return new WaitForSeconds(seconds);

        Managers.Resource.Destroy(go);

        MonsterController monster = Managers.Object.Spawn<MonsterController>(worldPos, info.DataId);
        monster.SetCellPos(cellPos, true);
        _spawnObjects.Add(monster);
    }

    private void SpawnBaseObjects()
    {
        foreach (ObjectSpawnInfo info in _spawnInfos)
        {
            Vector3 worldPos = info.WorldPos;
            Vector3Int cellPos = info.CellPos;
            
            if (Managers.Map.CanGo(null, cellPos) == false)
                return;
            
            switch (info.ObjectType)
            {
                case EObjectType.Monster:
                    StartSpawnDelay(worldPos, cellPos, info); // Temp > Base로 깔아둬야하는 Object들이기에 딜레이가 없어도 될 듯
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
            switch (obj.ObjectType)
            {
                case EObjectType.Monster:
                    Managers.Object.Despawn(obj as MonsterController);
                    break;
                case EObjectType.Npc:
                    Managers.Object.Despawn(obj as NpcController);
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

                _spawnInfos.Add(info);
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
    }
}