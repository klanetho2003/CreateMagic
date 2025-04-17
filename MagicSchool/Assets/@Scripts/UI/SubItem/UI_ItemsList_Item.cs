using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_ItemsList_Item : UI_Base
{
	enum Buttons
	{
        ItemButton,
	}

	enum Images
	{
        ItemImage,
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

    UI_ItemsListPopup _parentUI;

    Item item = null;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindImages(typeof(Images));

        GetButton((int)Buttons.ItemButton).gameObject.BindEvent(OnDragItemButton, OnUpItemButton, UIEvent.Drag, UIEvent.PointerUp);

        Refresh();

        return true;
	}

	public void SetInfo(int itemInstanceId, UI_ItemsListPopup parentUI)
	{
        transform.localScale = Vector3.one;

        _parentUI = parentUI;

        item = Managers.Inventory.GetItem(itemInstanceId);

        Refresh();
	}

	void Refresh()
	{
		if (_init == false)
			return;
		if (item == null)
			return;

		GetImage((int)Images.ItemImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.ItemDic[item.TemplateId].SpriteName);
		GetText((int)Texts.ItemCountText).text = $"{item.Count}";
	}

	void OnDragItemButton(PointerEventData evt)
	{
        if (item == null)
        {
            Debug.Log("아이템 존재 안 함");
            return;
        }

        if (item.EquipSlot == (int)Define.EEquipSlotType.UnknownItems)
            return;

        Debug.Log("On Drag");

        Vector2 touchPosition = evt.position;
        transform.position = touchPosition;

        /*if (item.IsEquippedItem())
            Managers.Inventory.UnEquipItem(item.InstanceId);
        else
            Managers.Inventory.EquipItem(item.InstanceId, Define.EEquipSlotType.Shift);*/
    }

    void OnUpItemButton(PointerEventData evt)
    {
        Debug.Log("On Up Button");

        _parentUI.Refresh();
    }
}
