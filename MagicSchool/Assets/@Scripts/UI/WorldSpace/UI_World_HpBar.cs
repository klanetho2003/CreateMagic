using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_World_HpBar : UI_Base
{
    private CreatureController _owner;
    RectTransform _rectTransform;
    Slider _slider;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _rectTransform = GetComponent<RectTransform>();
        _slider = GetComponent<Slider>();

        return true;
    }

    public void SetInfo(CreatureController owner)
    {
        _owner = owner;
        owner.OnHpChange += Refresh;

        _rectTransform.anchoredPosition = Vector2.zero;
    }

    public void Refresh(float ownerHp)
    {
        _slider.value = ownerHp / _owner.MaxHp.Value;
    }
}
