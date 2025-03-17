using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Define;
using Slider = UnityEngine.UI.Slider;

public class UI_GameScene_EachMpBar : UI_Base
{
    public Slider Slider { get; private set; }
    PlayerController _playerCache;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Slider = GetComponent<Slider>();
        _playerCache = Managers.Game.Player;

        return true;
    }

    public void SetInfo(UI_GameScene parent, float gaugeAmount)
    {
        transform.localScale = Vector3.one;

        Refresh(gaugeAmount);
    }

    public void Refresh(float gaugeAmount)
    {
        if (_init == false)
            return;

        Slider.value = gaugeAmount / _playerCache.CreatureData.MpGaugeAmount;
    }
}
