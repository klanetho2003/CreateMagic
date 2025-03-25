using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillCardItem : UI_Base
{
    private ItemData _data;

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

        BindEvent(skillCardBackgroundImage.gameObject, (Event) =>
        {
            //Managers.Game.Player.Skills.AddSkill<EgoSword>(/*transform.position*/);

            OnClick();
        });

        return true;
    }

    public void SetInfo(int itemTemplateId)
    {
        _data = Managers.Data.ItemDic[itemTemplateId];

        Refresh();
        
    }

    void Refresh()
    {
        // Temp Casting
        EquipmentData data = (EquipmentData)_data;

        // Text
        cardNameText.text = $"{data.Name}";
        skillDescriptionText.text = $"Player의 공격력을 {data.Damage} 상승 시킨다.";

        // Sprite
        skillImage.sprite = Managers.Resource.Load<Sprite>(_data.SpriteName);
    }

    public void OnClick()
    {
        // TEMP - For Debug Item > 카드 선택하는 화면으로 이동 필요
        Item tempItem = Managers.Inventory.MakeItem(_data.DataId);  // Inven에 들어감. TemplateData -> Data Sheet , SaveData -> DataBase_LocalSaveFile
        Managers.Inventory.EquipItem(tempItem.InstanceId);          // Equiped in GameScene

        Debug.Log($"Atk : {Managers.Game.Player.Atk.Value}");

        Managers.UI.ClosePopupUI();
    }
}
