using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillCardItem : UI_Base
{
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

    public void SetInfo(int templateID)
    {
        _templateID = templateID;

        Managers.Data.SkillDic.TryGetValue(templateID, out _skillData);
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindObject(typeof(GameObjects));

        cardNameText = GetText((int)Texts.CardNameText);
        skillDescriptionText = GetText((int)Texts.SkillDescriptionText);
        skillImage = GetImage((int)Images.SkillImage);
        skillCardBackgroundImage = GetGameObject((int)GameObjects.SkillCardBackgroundImage);

        RefreshTexts();
        skillImage.sprite = Managers.Resource.Load<Sprite>("EgoSwordIcon_01.sprite");

        BindEvent(skillCardBackgroundImage.gameObject, () =>
        {
            //Managers.Game.Player.Skills.AddSkill<EgoSword>(/*transform.position*/);
            Managers.UI.ClosePopUp();
        });

        return true;
    }

    void RefreshTexts()
    {
        cardNameText.text = "25년 행운 펀치";
        skillDescriptionText.text = "25년은 뭐든 할 수 있게 된다.";
    }

    public void OnClick() // ToDo
    {
        
    }
}
