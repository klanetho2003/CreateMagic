using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemsListPopup : UI_Popup
{
    #region Enum to Bind
    enum GameObjects
	{
		CloseArea,
        EquippedItemsList,
        InventoryItemsList,
        UnknownItemsList,
	}

	enum Texts
	{
        EquippedItemsCountText,
        InventoryItemsCountText,
        UnknownItemsCountText,
	}

	enum Buttons
	{
		CloseButton,
	}
    #endregion

    List<UI_ItemsList_Item> _equippedItems = new List<UI_ItemsList_Item>();
	List<UI_ItemsList_Item> _inventoryItems = new List<UI_ItemsList_Item>();
	List<UI_ItemsList_Item> _unknownItems = new List<UI_ItemsList_Item>();

	const int MAX_ITEM_COUNT = 100;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        {
            var parent = GetObject((int)GameObjects.EquippedItemsList).transform;
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                UI_ItemsList_Item item = Managers.UI.MakeSubItem<UI_ItemsList_Item>(parent);
                _equippedItems.Add(item);
            }
        }
        {
            var parent = GetObject((int)GameObjects.InventoryItemsList).transform;
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                UI_ItemsList_Item item = Managers.UI.MakeSubItem<UI_ItemsList_Item>(parent);
                _inventoryItems.Add(item);
            }
        }
        {
            var parent = GetObject((int)GameObjects.UnknownItemsList).transform;
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                UI_ItemsList_Item item = Managers.UI.MakeSubItem<UI_ItemsList_Item>(parent);
                _unknownItems.Add(item);
            }
        }

        // Refresh();

        return true;
	}

	public void SetInfo()
	{
		Refresh();
	}

	void Refresh()
	{
		if (_init == false)
			return;

        // To Do Item List Cache

        GetText((int)Texts.EquippedItemsCountText).text = $"{Managers.Inventory.GetEquippedItems().Count} / ??";
		GetText((int)Texts.InventoryItemsCountText).text = $"{Managers.Inventory.GetInventoryItems().Count} / ??";
		GetText((int)Texts.UnknownItemsCountText).text = $"{Managers.Inventory.GetUnknownItems().Count} / ??";

        Refresh_Item(_equippedItems, Managers.Inventory.GetEquippedItems());
        Refresh_Item(_inventoryItems, Managers.Inventory.GetInventoryItems());
        Refresh_Item(_unknownItems, Managers.Inventory.GetUnknownItems());
	}

	void Refresh_Item(List<UI_ItemsList_Item> uiItemList, List<Item> inventoryItemList)
	{
        for (int i = 0; i < uiItemList.Count; i++)
        {
            if (i < inventoryItemList.Count)
            {
                ItemSaveData item = inventoryItemList[i].SaveData;
                uiItemList[i].SetInfo(item.TemplateId, item.InstanceId);
                uiItemList[i].gameObject.SetActive(true);
            }
            else
            {
                uiItemList[i].gameObject.SetActive(false);
            }
        }
    }

	void OnClickCloseArea(PointerEventData evt)
	{
		Managers.UI.ClosePopupUI(this);
	}

	void OnClickCloseButton(PointerEventData evt)
	{
		Managers.UI.ClosePopupUI(this);
	}
}
