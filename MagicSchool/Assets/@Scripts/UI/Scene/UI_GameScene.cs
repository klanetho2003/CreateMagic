using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        WaveList,
    }

    List<UI_GameScene_WaveItem> _items = new List<UI_GameScene_WaveItem>();
    const int MAX_ITEM_COUNT = 30;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));

        _items.Clear();

        GameObject parent = GetObject((int)GameObjects.WaveList);
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_GameScene_WaveItem item = Managers.UI.MakeSubItem<UI_GameScene_WaveItem>(parent.transform);
            _items.Add(item);
        }

        Refresh();
        return true;
    }

    public void SetInfo()
    {
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        GameObject parent = GetObject((int)GameObjects.WaveList);
        List<EMonsterWaveType> waves = Managers.Map.StageTransition.CurrentStage.GetWaveTypesAll();

        for (int i = 0; i < _items.Count; i++)
        {
            if (i < waves.Count)
            {
                EMonsterWaveType wave = waves[i];
                _items[i].SetInfo(wave);
                _items[i].gameObject.SetActive(true);
            }
            else
            {
                _items[i].gameObject.SetActive(false);
            }
        }
    }
}
