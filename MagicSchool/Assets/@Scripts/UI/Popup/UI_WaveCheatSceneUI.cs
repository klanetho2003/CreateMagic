using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;
using static UnityEditor.Progress;

public class UI_WaveCheatSceneUI : UI_Scene
{
    enum GameObjects
    {
        WaveList,
    }

    List<UI_WaveItem> _items = new List<UI_WaveItem>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));

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

        _items.Clear();

        GameObject parent = GetObject((int)GameObjects.WaveList);

        List<EMonsterWaveType> waveData = Managers.Map.StageTransition.CurrentStage.GetWaveTypesAll();
        foreach (var wave in waveData)
        {
            UI_WaveItem item = Managers.UI.MakeSubItem<UI_WaveItem>(parent.transform);

            item.SetInfo(wave);

            _items.Add(item);
        }
    }
}
