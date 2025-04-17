using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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

        // To Do : 클릭 시 Item 해제
        // GetButton((int)Buttons.ItemButton).gameObject.BindEvent(OnDragItemButton, OnUpItemButton, UIEvent.Drag, UIEvent.PointerUp);

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
            GetImage((int)Images.EquipSlotImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.ItemDic[Slot_DataId].SpriteName);
        else
            GetImage((int)Images.EquipSlotImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.ItemDic[_item.TemplateId].SpriteName);
    }

    public void AttachItem(Item item)
    {
        _item = item;
    }
}
