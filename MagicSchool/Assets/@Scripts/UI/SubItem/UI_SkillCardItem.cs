using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillCardItem : UI_Base
{
    #region Binding

    enum Texts
    {
        CardNameText,
        SkillDescriptionText,
    }

    enum Images
    {
        SkillImage,
    }

    enum GameObjects
    {
        SkillCardBackgroundImage,
    }

    int _templateID;
    Data.SkillData _skillData;

    TMP_Text cardNameText;
    TMP_Text skillDescriptionText;

    Image skillImage;

    GameObject skillCardBackgroundImage;

    #endregion

    public void SetInfo(int templateID)
    {
        _templateID = templateID;

        Managers.Data.SkillDic.TryGetValue(templateID, out _skillData);
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));
        BindImages(typeof(Images));
        BindObjects(typeof(GameObjects));

        cardNameText = GetText((int)Texts.CardNameText);
        skillDescriptionText = GetText((int)Texts.SkillDescriptionText);
        skillImage = GetImage((int)Images.SkillImage);
        skillCardBackgroundImage = GetObject((int)GameObjects.SkillCardBackgroundImage);

        RefreshTexts();
        skillImage.sprite = Managers.Resource.Load<Sprite>("EgoSwordIcon_01.sprite");

        BindEvent(skillCardBackgroundImage.gameObject, (Event) =>
        {
            //Managers.Game.Player.Skills.AddSkill<EgoSword>(/*transform.position*/);
            Managers.UI.ClosePopupUI();
        });

        return true;
    }

    void RefreshTexts()
    {
        cardNameText.text = "¸Û¸Û¸Û";
        skillDescriptionText.text = "¸óÇå Àú´Â ÅÂµµÇÒ ¿¹Á¤ÀÔ´Ï´Ù.";
    }

    public void OnClick() // ToDo
    {
        
    }
}
