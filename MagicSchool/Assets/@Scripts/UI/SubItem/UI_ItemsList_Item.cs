using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_ItemsList_Item : UI_Base
{
    #region Bind Enum
    enum Buttons
	{
        ItemButton,
	}

	enum Images
	{
        ItemImage,
        ItemButton,
    }

	enum Texts
	{
        ItemMaxCountText,
        ItemCountText,
	}

	enum Sliders
	{
        ItemMaxCountSlider,
	}
    #endregion

    private RectTransform _rectTransform;

    private Item _item = null;

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

        _rectTransform = GetComponent<RectTransform>();

        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindImages(typeof(Images));

        GetButton((int)Buttons.ItemButton).gameObject.BindEvent(OnBeginDragItemButton, OnDragItemButton, OnUpItemButton, UIEvent.PointerDown, UIEvent.Drag, UIEvent.PointerUp);

        return true;
	}

	public void SetInfo(int itemInstanceId, UI_ItemsListPopup parentUI)
	{
        transform.localScale = Vector3.one;

        _item = Managers.Inventory.GetItem(itemInstanceId);

        Refresh();
	}

    void Refresh()
	{
		if (_init == false)
			return;
		if (_item == null)
			return;

        GetImage((int)Images.ItemImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.ItemDic[_item.TemplateId].SpriteName);
		GetText((int)Texts.ItemCountText).text = $"{_item.Count}";
	}

    private bool TryEquipToSlot(PointerEventData evt)
    {
        if (evt.pointerEnter == null) return false;

        var slot = evt.pointerEnter.GetComponentInParent<UI_ItemsList_EquipSlot>();
        if (slot == null) return false;
        slot.AttachItem(_item);

        Managers.Inventory.EquipItem(_item.InstanceId, slot.SlotType);

        return true;
    }

    #region EventHandling
    private Vector3 _originalPosition;
    void OnBeginDragItemButton(PointerEventData evt)
    {
        if (_item == null)
            return;
        if (_item.EquipSlot == (int)EEquipSlotType.UnknownItems)
            return;
        if (_item.IsEquippable() == false)
            return;

        _originalPosition = _rectTransform.position;

        GetImage((int)Images.ItemButton).raycastTarget = false;
    }

    void OnDragItemButton(PointerEventData evt)
	{
        if (_item == null)
            return;
        if (_item.EquipSlot == (int)EEquipSlotType.UnknownItems)
            return;
        if (_item.IsEquippable() == false)
            return;

        Vector2 touchPosition = evt.position;
        _rectTransform.position = touchPosition;
    }

    void OnUpItemButton(PointerEventData evt)
    {
        if (_item == null)
            return;
        if (_item.EquipSlot == (int)EEquipSlotType.UnknownItems)
            return;
        if (_item.IsEquippable() == false)
            return;

        if (!TryEquipToSlot(evt))
            _rectTransform.position = _originalPosition; // 실패 시 복귀

        GetImage((int)Images.ItemButton).raycastTarget = true;
    }
    #endregion
}
