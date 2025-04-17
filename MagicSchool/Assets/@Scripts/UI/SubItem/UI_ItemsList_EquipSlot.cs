using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
using static UnityEditor.Progress;

public class UI_ItemsList_EquipSlot : UI_Base
{
    enum Buttons
    {
        EquipSlotButton,
    }

    enum Images
    {
        EquipSlotImage,
    }

    enum Texts
    {
        ItemCountText,
        SlotInputKeyText,
    }

    const int Slot_DataId = 0;

    Item _item = null;
    public EEquipSlotType SlotType;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));
        BindImages(typeof(Images));

        GetButton((int)Buttons.EquipSlotButton).gameObject.BindEvent(OnClickSlotButton, UIEvent.Click);

        return true;
    }

    public void SetInfo(EEquipSlotType slotType)
    {
        SlotType = slotType;
        GetText((int)Texts.SlotInputKeyText).text = $"{SlotType}";

        Refresh();
    }

    public void Refresh()
    {
        if (_init == false)
            return;

        transform.localScale = Vector3.one;

        if (_item == null)
        {
            GetImage((int)Images.EquipSlotImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.ItemDic[Slot_DataId].SpriteName);
            GetText((int)Texts.ItemCountText).text = "N";
        }
        else
        {
            GetImage((int)Images.EquipSlotImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.ItemDic[_item.TemplateId].SpriteName);
            GetText((int)Texts.ItemCountText).text = $"{_item.Count}";
        }
            
    }

    public void AttachItem(Item item)
    {
        _item = item;
    }

    void OnClickSlotButton(PointerEventData evt)
    {
        if (_item == null)
            return;
        if (_item.IsEquippable() == false)
            return;

        Managers.Inventory.UnEquipItem(_item.InstanceId);
        _item = null;

        Refresh();
    }
}
