using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Define;

public class UI_GameScene_NavSkillItem : UI_Base
{
    PlayerSkillBase _skillBase;

    enum Texts
    {
        ValueText,
    }

    enum Images
    {
        SkillIconImage,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));
        BindImages(typeof(Images));

        return true;
    }

    public void SetInfo(PlayerSkillBase skill)
    {
        _skillBase = skill;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        GetText((int)Texts.ValueText).text = $"{_skillBase.PlayerSkillData.InputDescription}\n{_skillBase.SkillData.Name}";
        GetImage((int)Images.SkillIconImage).sprite = Managers.Resource.Load<Sprite>(_skillBase.SkillData.IconLabel);

        transform.localScale = Vector3.one;
    }
}
