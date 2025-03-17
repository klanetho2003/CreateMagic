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
    UI_GameScene _uiGameSceneCache { get; set; }

    EMpStateType _mpStateType;
    public EMpStateType MpStateType
    {
        get { return _mpStateType; }
        set
        {
            _mpStateType = value;

            /*switch (value)
            {
                case EMpStateType.Fill:
                    _uiGameSceneCache.FillMpBar = this;
                    _uiGameSceneCache.FullMpBars.Remove(this);
                    _uiGameSceneCache.NoneMpBars.Remove(this);
                    break;
                case EMpStateType.Full:
                    _uiGameSceneCache.FullMpBars.Push(this);
                    break;
                case EMpStateType.None:
                    _uiGameSceneCache.NoneMpBars.Add(this);
                    _uiGameSceneCache.FullMpBars.Remove(this);
                    break;
            }*/
        }
    }

    public Slider Slider { get; private set; }

    enum Sliders
    {
        UI_GameScene_EachMpBar,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Slider = GetComponent<Slider>();
        //BindSliders(typeof(Sliders));

        Refresh();

        return true;
    }

    public void SetInfo(UI_GameScene parent)
    {
        _uiGameSceneCache = parent;

        MpStateType = EMpStateType.None;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        transform.localScale = Vector3.one;
    }

    public void RefreshSlider()
    {
        Slider.value = Managers.Game.Player.CurrentMpGaugeAmount / 1;
    }

    public void ResetValue()
    {
        Slider.value = 0;
    }
}
