using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;

        var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, MAGICION01_ID);

        Managers.Map.LoadMap("BaseMap");
        Managers.Map.StageTransition.SetInfo();

        // Temp
        // Managers.Map.StageTransition.CurrentStage.CurrentWave = EMonsterWaveType.First;

        Managers.Map.MoveTo(player, Vector3Int.zero);

        // UI
        Managers.UI.ShowSceneUI<UI_GameScene>();

        // 메모리에만 들고 있던 Item Player에게 적용
        foreach (var item in Managers.Inventory.GetEquippedItems())
            item.ApplyItem(item, item.TemplateData.StatModType, player);
        Debug.Log($"Atk : {player.Atk.Value}");

        // Event
        Managers.Game.OnKillCountChanged -= HandleOnKillCountChanged;
        Managers.Game.OnKillCountChanged += HandleOnKillCountChanged;
        /*Managers.Game.OnJamCountChanged -= HandleOnJamCountChanged;
        Managers.Game.OnJamCountChanged += HandleOnJamCountChanged;*/

        // UI Cache
        Managers.UI.CacheAllPopups();

        return true;
    }

    public override void Clear()
    {

    }

    void HandleOnKillCountChanged(int killCount)
    {
        /*UI_GameScene uiGameScene = Managers.UI.SceneUI.GetComponent<UI_GameScene>();
        uiGameScene.SetKillCount(killCount);*/

        Stage currentStage = Managers.Map.StageTransition.CurrentStage;

        if (killCount >= currentStage.GetMonsterCountInWave(currentStage.CurrentWave))
        {
            /*StageType = Define.StageType.Boss;

            Managers.Object.DespawnAllMonsters();

            Vector2 spawnPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 4, 8);*/
            // Boss Spawn

            currentStage.CurrentWave++; // Start Next Wave
        }
    }

    private void OnDestroy()
    {
        if (Managers.Game != null)
        {
            // Managers.Game.OnJamCountChanged -= HandleOnJamCountChanged;
            Managers.Game.OnKillCountChanged -= HandleOnKillCountChanged;
        }
    }













    SpawningPool _spawningPool;

    /*Define.StageType _stageType;
    public Define.StageType StageType
    {
        get { return _stageType; }
        set
        {
            _stageType = value;

            if (_spawningPool != null)
            {
                switch (value)
                {
                    case Define.StageType.Normal:
                        _spawningPool.Stopped = false;
                        break;
                    case Define.StageType.Boss:
                        _spawningPool.Stopped = true;
                        break;
                }
            }
        }
    }*/


    int _collectedJamCount = 0;
    int _remainingTotalJamCount = 10;
    void HandleOnJamCountChanged(int jamCount)
    {
        _collectedJamCount++;

        if (_collectedJamCount == _remainingTotalJamCount)
        {
            Managers.UI.ShowPopupUI<UI_SkillSelectPopup>();
            _collectedJamCount = 0;
            _remainingTotalJamCount *= 2;
        }

        UI_GameScene_Temp uiGameScene = Managers.UI.SceneUI.GetComponent<UI_GameScene_Temp>();
        uiGameScene.SetJamCountRatio((float)jamCount / _remainingTotalJamCount);
    }
}
