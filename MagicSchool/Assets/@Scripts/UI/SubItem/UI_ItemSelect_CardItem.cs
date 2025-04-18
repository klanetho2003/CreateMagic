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
        RewardDescriptionText,
    }

    enum Images
    {
        RewardImage,
    }

    enum GameObjects
    {
        RewardCardBackgroundImage,
    }

    TMP_Text _cardNameText;
    TMP_Text _rewardDescriptionText;

    Image _rewardImage;

    GameObject _rewardCardBackgroundImage;

    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));
        BindImages(typeof(Images));
        BindObjects(typeof(GameObjects));

        _cardNameText = GetText((int)Texts.CardNameText);
        _rewardDescriptionText = GetText((int)Texts.RewardDescriptionText);
        _rewardImage = GetImage((int)Images.RewardImage);
        _rewardCardBackgroundImage = GetObject((int)GameObjects.RewardCardBackgroundImage);

        BindEvent(_rewardCardBackgroundImage.gameObject, (Event) =>
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
        transform.localScale = Vector3.one;

        // Name
        _rewardDescriptionText.text = $"{_data.Name}";
        // Description
        string descriptionText = Managers.GetText(_data.RewardDescriptionId);
        _rewardDescriptionText.text = descriptionText;

        // Sprite
        _rewardImage.sprite = Managers.Resource.Load<Sprite>(_data.SpriteName);

    }

    public void OnClick()
    {
        Item item = Managers.Inventory.GetUnknownItem(_data.DataId);
        if (item == null)
            item = Managers.Inventory.GetItemByTemplateId(_data.DataId);

        // Item ¾ò±â
        Managers.Inventory.GainItem(item.InstanceId, Define.EEquipSlotType.Inventory);

        Managers.UI.ClosePopupUI();
    }
}
