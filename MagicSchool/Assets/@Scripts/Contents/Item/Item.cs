using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Item
{
    public ItemSaveData SaveData { get; set; } = new ItemSaveData();
    public List<StatModifier> statModifiers { get; } = new List<StatModifier>();

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
        return true;
    }

    public static Item MakeItem(ItemSaveData itemInfo)
    {
        if (Managers.Data.ItemDic.TryGetValue(itemInfo.TemplateId, out ItemData itemData) == false)
            return null;

        Item item = null;

        switch (itemData.Type)
        {
            case EItemType.Artifact:
                item = new Artifact(itemInfo.TemplateId);
                break;
            case EItemType.Weapon:
                item = new Equipment(itemInfo.TemplateId);
                break;
            case EItemType.Armor:
                item = new Equipment(itemInfo.TemplateId);
                break;
            case EItemType.Potion:
                item = new Consumable(itemInfo.TemplateId);
                break;
            case EItemType.Scroll:
                item = new Consumable(itemInfo.TemplateId);
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

    // ActiveItem으로 옮길 것
    public static bool RemoveItemInDic_Temp(ItemSaveData itemInfo)
    {
        // Inventory에서 보여져야 하기에 Item Data를 담고 있는 Dictionary에서는 지우면 안됨!!!
        // return Managers.Data.ItemDic.Remove(itemInfo.TemplateId);
        return false;
    }

    #region Helpers

    // Active Item Class로 옮기는 것을 고려
    public bool IsEquippable()
    {
        return GetEquipItemEquipSlot() != EEquipSlotType.None;
    }
    // Active Item Class로 옮기는 것을 고려
    public EEquipSlotType GetEquipItemEquipSlot()
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
    }






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
    #endregion

    public void ClearStatModifier()
    {
        // To Do
    }
}



public class Consumable : Item
{
    public double Value { get; private set; }

    public Consumable(int templateId) : base(templateId)
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (TemplateData == null)
            return false;

        if (TemplateData.Type != EItemType.Potion && TemplateData.Type != EItemType.Scroll)
            return false;

        ConsumableData data = (ConsumableData)TemplateData;
        {
            Value = data.Value;
        }

        return true;
    }
}
