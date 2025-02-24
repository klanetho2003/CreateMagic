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

        //Managers.UI.ShowSceneUI<UI_GameScene>();

        var player = Managers.Object.Spawn<PlayerController>(Vector3.zero, MAGICION01_ID);

        //Managers.Map.LoadMap("@Map_Prototype_Inside");
        Managers.Map.LoadMap("BaseMap");

        Managers.Map.MoveTo(player, Vector3Int.zero);

        /*Managers.Game.OnJamCountChanged -= HandleOnJamCountChanged;
        Managers.Game.OnJamCountChanged += HandleOnJamCountChanged;
        Managers.Game.OnKillCountChanged -= HandleOnKillCountChanged;
        Managers.Game.OnKillCountChanged += HandleOnKillCountChanged;*/

        return true;
    }

    public override void Clear()
    {

    }













    SpawningPool _spawningPool;

    Define.StageType _stageType;
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
    }


    int _collectedJamCount = 0;
    int _remainingTotalJamCount = 10;
    void HandleOnJamCountChanged(int jamCount)
    {
        _collectedJamCount++;

        if (_collectedJamCount == _remainingTotalJamCount)
        {
            Managers.UI.ShowPopUI<UI_SkillSelectPopup>();
            _collectedJamCount = 0;
            _remainingTotalJamCount *= 2;
        }

        UI_GameScene uiGameScene = Managers.UI.SceneUI.GetComponent<UI_GameScene>();
        uiGameScene.SetJamCountRatio((float)jamCount / _remainingTotalJamCount);
    }

    void HandleOnKillCountChanged(int killCount)
    {
        UI_GameScene uiGameScene = Managers.UI.SceneUI.GetComponent<UI_GameScene>();
        uiGameScene.SetKillCount(killCount);

        if (killCount == 15)
        {
            StageType = Define.StageType.Boss;

            Managers.Object.DespawnAllMonsters();

            Vector2 spawnPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 4, 8);
            // Boss Spawn
        }
    }

    private void OnDestroy()
    {
        if (Managers.Game != null)
        {
            Managers.Game.OnJamCountChanged -= HandleOnJamCountChanged;
            Managers.Game.OnKillCountChanged -= HandleOnKillCountChanged;
        }
    }
}
