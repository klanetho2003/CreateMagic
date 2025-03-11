using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Define;

public class UI_GameScene_NavSkillItem : UI_Base
{
    SkillBase _skillBase;

    enum Texts
    {
        ValueText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));

        Refresh();

        return true;
    }

    public void SetInfo(SkillBase skill)
    {
        _skillBase = skill;

        GetText((int)Texts.ValueText).text = $"{skill.SkillData.Name}";
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;
    }
}
