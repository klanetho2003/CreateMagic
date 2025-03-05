using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene_Temp : UI_Scene
{
    enum Texts
    {
        KillValueText,
    }

    enum Sliders
    {
        ExpSliderObject,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region ObjectBind
        BindTexts(typeof(Texts));
        Bind<Slider>(typeof(Sliders));
        #endregion

        return true;
    }

    public void SetJamCountRatio(float ratio)
    {
        Slider slider = Get<Slider>((int)Sliders.ExpSliderObject);
        slider.value = ratio;
    }

    public void SetKillCount(int killCount)
    {
        TMP_Text killValueText = GetText((int)Texts.KillValueText);
        killValueText.text = $"{killCount}";
    }
}
