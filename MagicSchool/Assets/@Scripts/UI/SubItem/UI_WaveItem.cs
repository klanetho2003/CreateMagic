using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_WaveItem : UI_Base
{
    private EMonsterWaveType _waveType;

    enum Texts
    {
        NameText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));

        gameObject.BindEvent(OnClickWaveItem);

        Refresh();

        return true;
    }

    public void SetInfo(EMonsterWaveType waveType)
    {
        _waveType = waveType;

        GetText((int)Texts.NameText).text = $"{waveType}";
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;
    }

    void OnClickWaveItem(PointerEventData evt)
    {
        Debug.Log("OnClickWaveItem");

        Managers.Map.StageTransition.CurrentStage.StopAllCoroutines();
        Managers.Object.DespawnAllMonsters();
        Managers.Map.StageTransition.CurrentStage.StartWave(_waveType, true);
    }
}
