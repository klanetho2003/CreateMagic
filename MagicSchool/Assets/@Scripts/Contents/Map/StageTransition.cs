using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageTransition : MonoBehaviour
{
    public List<Stage> Stages = new List<Stage>();
    public Stage CurrentStage { get; set; }
    public int CurrentStageIndex { get; private set; } = -1;
    private PlayerController _leader;

    #region Init

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

    #endregion

    public void SetInfo()
    {
        int currentMapIndex = 0;

        for (int i = 0; i < Stages.Count; i++)
        {
            Stages[i].SetInfo(i);  
            
            if (Stages[i].StartSpawnInfo.WorldPos != Vector3.zero)
            {
                currentMapIndex = i;
            }
        }

        OnMapChanged(currentMapIndex);
	}

    public void CheckMapChanged(Vector3 position)
    {
        if (CurrentStage.IsPointInStage(position) == false)
        {
            int stageIndex = GetStageIndex(position);
            OnMapChanged(stageIndex);
        }
    }

    private int GetStageIndex(Vector3 position)
    {
        for (int i = 0; i < Stages.Count; i++)
        {
            if(Stages[i].IsPointInStage(position))
            {
                return i;
            }
        }

        Debug.LogError("Cannot Find CurrentMapZone");
        return -1;
    }

    public void OnMapChanged(int newMapIndex)
    {
        CurrentStageIndex = newMapIndex;
        CurrentStage = Stages[CurrentStageIndex];
        
        LoadMapsAround(newMapIndex);
        UnloadOtherMaps(newMapIndex);
    }

    private void LoadMapsAround(int mapIndex)
    {
        // 이전, 현재, 다음 맵을 로드
        for (int i = mapIndex - 1; i <= mapIndex + 1; i++)
        {
            if (i > -1 && i < Stages.Count) 
            {
                Debug.Log($"{i} Stage Load -> {Stages[i].name}");
                Stages[i].LoadStage();
            }
        }
    }

    private void UnloadOtherMaps(int mapIndex)
    {
        for (int i = 0; i < Stages.Count; i++)
        {
            if (i < mapIndex - 1 || i > mapIndex + 1)
            {
                Debug.Log($"{i} Stage UnLoad -> {Stages[i].name}");
                Stages[i].UnLoadStage();
            }
        }
    }
}
