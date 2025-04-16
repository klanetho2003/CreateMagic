using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemSelect_CardItem : UI_Base
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
            OnClick();
        });

        return true;
    }

    public void SetInfo(ItemData itemData)
    {
        _data = itemData;

        Refresh();
    }

    void Refresh()
    {
        transform.localScale = new Vector3(1.2f, 1.2f, 0);

        #region Temp Code - Casting And View Item
        // Temp Casting
        if (_data.ItemGroupType == Define.EItemGroupType.StatBoost)
        {
            StatBoostData data = (StatBoostData)_data;

            // Text
            cardNameText.text = $"{data.Name}";
            skillDescriptionText.text = $"Player의 공격력을 {data.Damage} 상승 시킨다.";
        }
        else if (_data.ItemGroupType == Define.EItemGroupType.Equipment)
        {
            EquipmentData data = (EquipmentData)_data;

            // Text
            cardNameText.text = $"{data.Name}";
            skillDescriptionText.text = $"장착 시험용 Temp Item 입니다.";
        }
        else if (_data.ItemGroupType == Define.EItemGroupType.Consumable)
        {
            ConsumableData data = (ConsumableData)_data;

            // Text
            cardNameText.text = $"{data.Name}";
            skillDescriptionText.text = $"Consumable";
        }

        // Sprite
        skillImage.sprite = Managers.Resource.Load<Sprite>(_data.SpriteName);
        #endregion

    }

    public void OnClick()
    {
        Item item = Managers.Inventory.GetUnknownItem(_data.DataId);
        if (item == null)
            item = Managers.Inventory.GetItemByTemplateId(_data.DataId);

        // Item 얻기
        Managers.Inventory.GainItem(item.InstanceId, Define.EEquipSlotType.Inventory);

        Managers.UI.ClosePopupUI();
    }
}
