using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Item
{
    public ItemSaveData SaveData { get; set; } = new ItemSaveData();

    #region Save Datas
    public int InstanceId
    {
        get { return SaveData.InstanceId; }
        set { SaveData.InstanceId = value; }
    }

    public long DbId
    {
        get { return SaveData.DbId; }
    }

    public int TemplateId
    {
        get { return SaveData.TemplateId; }
        set { SaveData.TemplateId = value; }
    }

    public int Count
    {
        get { return SaveData.Count; }
        set { SaveData.Count = value; }
    }

    public int EquipSlot
    {
        get { return SaveData.EquipSlot; }
        set { SaveData.EquipSlot = value; }
    }
    #endregion

    // Caching
    public Data.ItemData TemplateData 
    {
        get { return Managers.Data.ItemDic[TemplateId]; }
    }

    public EItemType ItemType { get; private set; }
    public EItemSubType SubType { get; private set; }

    // 생성자
    public Item(int templateId)
    {
        TemplateId = templateId;
        ItemType = TemplateData.Type;
        SubType = TemplateData.SubType;
    }

    public virtual bool Init()
    {
        if (TemplateData == null)
            return false;

        return true;
    }

    public static Item MakeItem(ItemSaveData itemInfo)
    {
        if (Managers.Data.ItemDic.TryGetValue(itemInfo.TemplateId, out ItemData itemData) == false)
            return null;

        Item item = null;

        switch (itemData.Type)
        {
            case EItemType.StatBoost:
                item = new StatBoost(itemInfo.TemplateId);
                break;

            case EItemType.ItemSkill:
                item = new ItemSkill(itemInfo.TemplateId);
                break;
            case EItemType.Potion:
                item = new Potion(itemInfo.TemplateId);
                break;
            case EItemType.Scroll:
                item = new Scroll(itemInfo.TemplateId);
                break;
        }

        if (item != null)
        {
            item.SaveData = itemInfo;
            item.InstanceId = itemInfo.InstanceId;
            item.Count = itemInfo.Count;
        }

        return item;
    }

    public bool TryChangeCount(int amount)
    {
        int tempCount = Count + amount;

        // 음수가 되는 경우 false
        if (tempCount < 0)
            return false;
            
        // 최대값을 넘긴 경우 조정 후 true
        Count = Mathf.Min(tempCount, TemplateData.MaxCount);
        return true;
    }

    #region Helpers
    public bool IsMaxCount()
    {
        if (Count > TemplateData.MaxCount || Count < 0)
            Debug.LogWarning($"Count_({Count}) , MaxCount_({TemplateData.MaxCount})");

        return Count == TemplateData.MaxCount;
    }

    public bool IsEquippable()
    {
        return ItemType != EItemType.StatBoost;
    }

    // 특정 Item이 어디에 장착되어 있는지 알려주는 함수
    /*public EEquipSlotType GetEquipItemEquipSlot()
    {
        if (ItemType == EItemType.Weapon)
            return EEquipSlotType.Weapon;

        if (ItemType == EItemType.Armor)
        {
            switch (SubType)
            {
                case EItemSubType.Helmet:
                    return EEquipSlotType.Helmet;
                case EItemSubType.Armor:
                    return EEquipSlotType.Armor;
                case EItemSubType.Shield:
                    return EEquipSlotType.Shield;
                case EItemSubType.Gloves:
                    return EEquipSlotType.Gloves;
                case EItemSubType.Shoes:
                    return EEquipSlotType.Shoes;
            }
        }

        return EEquipSlotType.None;
    }*/
    #endregion

    #region Overrides
    public virtual void ApplyItemAbility(EStatModType statModType, CreatureController target)
    {
        if (target.IsValid() == false)
            return;

        // override
    }

    public virtual void RemoveItemAbility(CreatureController target)
    {
        if (target.IsValid() == false)
            return;

        // override
    }

    public virtual void UseItem(CreatureController target)
    {
        if (target.IsValid() == false)
            return;

        // override
    }
    #endregion

    public bool IsEquippedItem()
    {
        return SaveData.EquipSlot > (int)EEquipSlotType.None && SaveData.EquipSlot < (int)EEquipSlotType.EquipMax;
    }

    public bool IsInInventory()
    {
        return SaveData.EquipSlot == (int)EEquipSlotType.Inventory;
    }

    public bool IsInUnknownItems()
    {
        return SaveData.EquipSlot == (int)EEquipSlotType.UnknownItems;
    }

    public void ClearStatModifier()
    {
        // To Do
    }
}
