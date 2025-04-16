using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class InventoryManager
{
    PlayerController _player { get { return Managers.Game.Player; } }

    public readonly int DEFAULT_INVENTORY_SLOT_COUNT = 30;

    public List<Item> AllItems { get; } = new List<Item>();

    // Cache
    Dictionary<int /*EquipSlot*/, Item> EquippedItems = new Dictionary<int, Item>(); // ��� �κ�
    List<Item> InventoryItems = new List<Item>(); // �κ�
    Dictionary<int /*IntanceId*/, Item> UnknownItems = new Dictionary<int, Item>(); // ����X
    Dictionary<EItemGrade, HashSet<int/*TemplateId*/>> RewardItems = new Dictionary<EItemGrade, HashSet<int>>() // Stage Clear �� ������ �� �ִ� Item
    {
        { EItemGrade.Normal, new HashSet<int>() },
        { EItemGrade.Rare, new HashSet<int>() },
        { EItemGrade.Legendary, new HashSet<int>() },
        { EItemGrade.Epic, new HashSet<int>() },
        // �߰��Ǹ� ��޺��� List�� �߰��� ��
    };

    // Evnet
    public Action OnItemSlotChange;

    #region In Init Game or Load Game
    // �������� ���� Item Make
    public Item MakeItem(int itemTemplateId, EEquipSlotType equipSlot = EEquipSlotType.UnknownItems, int count = 0)
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
            EquipSlot = (int)equipSlot,
            EnchantCount = 0,
        };

        return AddItem(saveData);
    }

    // SaveFile�� �ִ� Item�� In Game Data�� ��ȯ
    public Item AddItem(ItemSaveData itemInfo)
    {
        Item item = Item.MakeItem(itemInfo);
        if (item == null)
            return null;

        // �ߺ� Check
        if (GetItem(item.InstanceId) != null)
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
            UnknownItems.Add(item.InstanceId, item);
        }
        
        // Make Reward Data
        if (item.Count < item.TemplateData.MaxCount)
            RewardItems[item.TemplateData.Grade].Add(item.TemplateId);

        AllItems.Add(item);

        // UI Refresh
        OnItemSlotChange?.Invoke();

        // Item.RemoveItemInDic(itemInfo); // �ٽ� ���� �� ������ Dictionay���� ���� -> RewardItem�� ���� �ʿ�

        return item;
    }
    #endregion

    // Item ȹ��
    public void GainItem(int instanceId, EEquipSlotType equipSlotType)
    {
        Item item = GetItem(instanceId);
        if (item == null)
        {
            Debug.Log("������������� in AllItems");
            return;
        }

        // Item Count ++
        if (item.TryChangeCount(1)) // To Do Data Parsng
        {
            if (item.IsMaxCount())
                RewardItems[item.TemplateData.Grade].Remove(item.TemplateId);
        }

        // ������ Gain in Inventory
        if (item.EquipSlot == (int)EEquipSlotType.None)
        {
            item.EquipSlot = (int)EEquipSlotType.Inventory; // Save Data ����
            InventoryItems.Add(item);                       // In Game Dictionary�� ����
        }

        // Item Remove in UnknownItems
        UnknownItems.Remove(item.InstanceId);
        // Item Remove in RewardItems
        RewardItems[item.TemplateData.Grade].Remove(item.TemplateId);

        if (_player.IsValid())
            item.ApplyItemAbility(item.TemplateData.StatModType, _player);

        // UI Refresh
        OnItemSlotChange?.Invoke();
    }

    // Item ������
    public void WasteItem(int instanceId)
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
            UnknownItems.Remove(item.InstanceId);
        }

        if (GetItemInRewards(item.TemplateData.Grade, item.InstanceId) == null)
            RewardItems[item.TemplateData.Grade].Add(item.TemplateId);

        AllItems.Remove(item);

        // UI Refresh
        OnItemSlotChange?.Invoke();
    }

    #region Ż��
    // Item ����
    public void EquipItem(int instanceId, EEquipSlotType equipSlotType)
    {
        Item item = InventoryItems.Find(x => x.SaveData.InstanceId == instanceId);
        if (item == null)
        {
            Debug.Log("�������������");
            return;
        }

        // ���� �Ұ� Item �Ǻ�
        if (item.IsEquippable() == false)
            return;

        // ���� ������ ����
        if (EquippedItems.TryGetValue((int)equipSlotType, out Item prev))
            UnEquipItem(prev.InstanceId);

        // ������ ����
        item.EquipSlot = (int)equipSlotType;        // Save Data ����
        EquippedItems[(int)equipSlotType] = item;   // In Game Dictionary�� ����

        // ������ Remove in Inventory
        InventoryItems.Remove(item);

        // UI Refresh
        OnItemSlotChange?.Invoke();
    }

    // Item ���� ����
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

        // UI Refresh
        OnItemSlotChange?.Invoke();
    }
    #endregion

    // UI �۾� ������ �� ���� ����� Helper
    #region Helper
    // All
    public Item GetItem(int instanceId)
    {
        return AllItems.Find(item => item.InstanceId == instanceId);
    }

    public Item GetItemByTemplateId(int templateId)
    {
        return AllItems.Find(item => item.TemplateId == templateId);
    }

    // RewardItems�� ��ȸ�ϸ鼭 �Ű������� ���� ���� TemplateId�� ���� Item�� �ִ��� Ȯ���غ���

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
    public Item GetUnknownItem(int templateId)
    {
        foreach (Item item in UnknownItems.Values)
        {
            if (item.TemplateId == templateId)
                return item;
        }

        return null;
    }

    public List<Item> GetUnknownItems()
    {
        return UnknownItems.Values.ToList();
    }

    public List<ItemSaveData> GetUnknownItemInfos()
    {
        List<ItemSaveData> itemSaveDatas = new List<ItemSaveData>();

        foreach (Item item in UnknownItems.Values)
            itemSaveDatas.Add(item.SaveData);

        return itemSaveDatas;
    }

    /*public List<ItemSaveData> GetUnknownItemInfosOrderbyGrade()
    {
        return UnknownItems.OrderByDescending(y => (int)y.TemplateData.Grade)
                                    .ThenBy(y => (int)y.TemplateId)
                                    .Select(x => x.SaveData)
                                    .ToList();
    }*/

    // RewardsItems
    public Item GetItemInRewards(EItemGrade itemGrade, int hashTemplateId)
    {
        if (RewardItems[itemGrade].TryGetValue(hashTemplateId, out int itemTemplateId) == false)
            return null;

        return GetItemByTemplateId(itemTemplateId);
    }

    public List<int> GetRewardItemsByGrade(EItemGrade itemGrade)
    {
        return RewardItems[itemGrade].ToList(); // return TemplateIds
    }
    #endregion

    public void Clear()
    {
        AllItems.Clear();

        EquippedItems.Clear();
        InventoryItems.Clear();
        UnknownItems.Clear();
    }
}
