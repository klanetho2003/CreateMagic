using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class InventoryManager
{
    public readonly int DEFAULT_INVENTORY_SLOT_COUNT = 30;

    public List<Item> AllItems { get; } = new List<Item>();

    PlayerController _player { get { return Managers.Game.Player; } }

    // Cache
    Dictionary<int /*EquipSlot*/, Item> EquippedItems = new Dictionary<int, Item>(); // 장비 인벤
    List<Item> InventoryItems = new List<Item>(); // 인벤
    List<Item> UnknownItems = new List<Item>(); // 등장X

    // 보유하지 않은 Item Make
    public Item MakeItem(int itemTemplateId, int count = 1)
    {
        int itemDbId = Managers.Game.GenerateItemDbId();

        if (Managers.Data.ItemDic.TryGetValue(itemTemplateId, out ItemData itemData) == false)
            return null;

        ItemSaveData saveData = new ItemSaveData()
        {
            InstanceId = itemDbId,
            DbId = itemDbId,
            TemplateId = itemTemplateId,
            Count = count,
            EquipSlot = (int)EEquipSlotType.Inventory, // temp - 기본적으론 Inven에 들어가지 않을까
            EnchantCount = 0,
        };

        return AddItem(saveData);
    }

    // Memory에만 있는 Item을 실질적으로 적용
    public Item AddItem(ItemSaveData itemInfo)
    {
        Item item = Item.MakeItem(itemInfo);
        if (item == null)
            return null;

        if (item.IsEquippedItem())
        {
            EquippedItems.Add(item.SaveData.EquipSlot, item);
        }
        else if (item.IsInInventory())
        {
            InventoryItems.Add(item);
        }
        else if (item.IsInUnknownItems())
        {
            UnknownItems.Add(item);
        }

        AllItems.Add(item);

        item.ApplyItemAbility(item.TemplateData.StatModType, _player);
        // Item.RemoveItemInDic(itemInfo); // 다시 얻을 수 없도록 Dictionay에서 삭제 -> RewardItem과 연계 필요

        return item;
    }

    // Item 완전 삭제 Method - ex 버리기
    public void RemoveItem(int instanceId)
    {
        Item item = AllItems.Find(x => x.SaveData.InstanceId == instanceId);
        if (item == null)
            return;

        if (item.IsEquippedItem())
        {
            EquippedItems.Remove(item.SaveData.EquipSlot);
        }
        else if (item.IsInInventory())
        {
            InventoryItems.Remove(item);
        }
        else if (item.IsInUnknownItems())
        {
            UnknownItems.Remove(item);
        }

        AllItems.Remove(item);
    }

    // Item 장착
    public void EquipItem(int instanceId)
    {
        Item item = InventoryItems.Find(x => x.SaveData.InstanceId == instanceId);
        if (item == null)
        {
            Debug.Log("아이템존재안함");
            return;
        }

        // 장착 불가 Item 판별
        EEquipSlotType equipSlotType = item.GetEquipItemEquipSlot();
        if (equipSlotType == EEquipSlotType.None)
            return;

        // 기존 아이템 해제
        if (EquippedItems.TryGetValue((int)equipSlotType, out Item prev))
            UnEquipItem(prev.InstanceId);

        // 아이템 장착
        item.EquipSlot = (int)equipSlotType;
        EquippedItems[(int)equipSlotType] = item;

        // CallBack - UI
    }

    // Item 장착 해제
    public void UnEquipItem(int instanceId, bool checkFull = true)
    {
        var item = EquippedItems.Values.Where(x => x.InstanceId == instanceId).FirstOrDefault();
        if (item == null)
            return;

        // TODO

        if (checkFull && IsInventoryFull())
            return;

        EquippedItems.Remove((int)item.EquipSlot);

        item.EquipSlot = (int)EEquipSlotType.Inventory;
        InventoryItems.Add(item);

        // Item Stat Remove
        // item.RemoveItemAbility(_player);

        // CallBack - UI
    }

    public void Clear()
    {
        AllItems.Clear();

        EquippedItems.Clear();
        InventoryItems.Clear();
        UnknownItems.Clear();
    }

    // UI 작업 진행할 때 많이 사용할 Helper
    #region Helper
    public Item GetItem(int instanceId)
    {
        return AllItems.Find(item => item.InstanceId == instanceId);
    }

    // Equip
    public Item GetEquippedItem(EEquipSlotType equipSlotType)
    {
        EquippedItems.TryGetValue((int)equipSlotType, out Item item);

        return item;
    }

    public Item GetEquippedItem(int instanceId)
    {
        return EquippedItems.Values.Where(x => x.InstanceId == instanceId).FirstOrDefault();
    }

    public Item GetEquippedItemBySubType(EItemSubType subType)
    {
        return EquippedItems.Values.Where(x => x.SubType == subType).FirstOrDefault();
    }

    public List<Item> GetEquippedItems()
    {
        return EquippedItems.Values.ToList();
    }

    public List<ItemSaveData> GetEquippedItemInfos()
    {
        return EquippedItems.Values.Select(x => x.SaveData).ToList();
    }

    // Inventory
    public Item GetItemInInventory(int instanceId)
    {
        return InventoryItems.Find(x => x.SaveData.InstanceId == instanceId);
    }

    public bool IsInventoryFull()
    {
        return InventoryItems.Count >= InventorySlotCount();
    }

    public int InventorySlotCount()
    {
        return DEFAULT_INVENTORY_SLOT_COUNT;
    }

    public List<Item> GetInventoryItems()
    {
        return InventoryItems.ToList();
    }

    public List<ItemSaveData> GetInventoryItemInfos()
    {
        return InventoryItems.Select(x => x.SaveData).ToList();
    }

    public List<ItemSaveData> GetInventoryItemInfosOrderbyGrade()
    {
        return InventoryItems.OrderByDescending(y => (int)y.TemplateData.Grade)
                        .ThenBy(y => (int)y.TemplateId)
                        .Select(x => x.SaveData)
                        .ToList();
    }

    // Unknown
    public List<Item> GetUnknownItems()
    {
        return UnknownItems.ToList();
    }

    public List<ItemSaveData> GetUnknownItemInfos()
    {
        return UnknownItems.Select(x => x.SaveData).ToList();
    }

    public List<ItemSaveData> GetUnknownItemInfosOrderbyGrade()
    {
        return UnknownItems.OrderByDescending(y => (int)y.TemplateData.Grade)
                                    .ThenBy(y => (int)y.TemplateId)
                                    .Select(x => x.SaveData)
                                    .ToList();
    }
    #endregion
}
