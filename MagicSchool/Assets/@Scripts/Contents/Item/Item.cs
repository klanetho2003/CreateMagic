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

    // ������
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

    public static bool RemoveItemInDic_Temp(ItemSaveData itemInfo)
    {
        // Inventory���� �������� �ϱ⿡ Item Data�� ��� �ִ� Dictionary������ ����� �ȵ�!!!
        // return Managers.Data.ItemDic.Remove(itemInfo.TemplateId);
        return false;
    }

    #region Helpers
    public bool IsEquippable()
    {
        return GetEquipItemEquipSlot() != EEquipSlotType.None;
    }

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

public class Artifact : Item
{
    public float Damage { get; private set; }
    public float Defence { get; private set; }
    public double Speed { get; private set; }

    protected Data.ArtifactData ArtifactData { get { return (Data.ArtifactData)TemplateData; } }

    public Artifact(int templateId) : base(templateId)
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (TemplateData == null)
            return false;

        if (TemplateData.Type != EItemType.Artifact)
            return false;

        ArtifactData data = (ArtifactData)TemplateData;
        {
            Damage = data.Damage;
            Defence = data.Defence;
            Speed = data.Speed;
        }

        return true;
    }

    //  Temp, To DO : � Stat�� � StatModType ��ŭ ����ϴ��� Class�� ���� ��, List�� �����ؾ��� ��
    public override void ApplyItemAbility(EStatModType statModType, CreatureController target)
    {
        base.ApplyItemAbility(statModType, target);

        // Inventory�� ���� ����ΰ�? (��, ȹ���� ����ΰ�?)
        if (this.IsInInventory() == false)
            return;

        if (this.IsHaveAtkWeight())
        {
            StatModifier Atk = new StatModifier(this.Damage, statModType);
            target.Atk.AddModifier(Atk);
            statModifiers.Add(Atk);
        }

        if (this.IsHaveDefWeight())
        {
            /*StatModifier Def = new StatModifier(EItem.Defence, statModType);
            target.Atk.AddModifier(Def);*/
        }

        if (this.IsHaveSpeedWeight())
        {
            StatModifier speed = new StatModifier((float)this.Speed, statModType);
            target.MoveSpeed.AddModifier(speed);
            statModifiers.Add(speed);
        }
    }

    public override void RemoveItemAbility(CreatureController target)
    {
        base.RemoveItemAbility(target);

        foreach (var modifier in statModifiers)
        {
            // ���ŵ����� true �Ұ��ϸ� false
            bool tempDebug1 = target.Atk.RemoveModifier(modifier);
            bool tempDebug2 = target.MoveSpeed.RemoveModifier(modifier);
        }
    }

    #region Helpers
    public bool IsHaveAtkWeight()
    {
        return Damage != 0;
    }
    public bool IsHaveDefWeight()
    {
        return Defence != 0;
    }
    public bool IsHaveSpeedWeight()
    {
        return Speed != 0;
    }
    #endregion
}

public class Equipment : Item
{
    public Equipment(int templateId) : base(templateId)
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (TemplateData == null)
            return false;

        if (TemplateData.Type != EItemType.Weapon && TemplateData.Type != EItemType.Armor)
            return false;

        // Equipment Data Parsing
        /*ArtifactData data = (ArtifactData)TemplateData;
        {
            Damage = data.Damage;
            Defence = data.Defence;
            Speed = data.Speed;
        }*/

        return true;
    }
    public override void ApplyItemAbility(EStatModType statModType, CreatureController target)
    {
        base.ApplyItemAbility(statModType, target);

        // ������ ����ΰ�?
        if (this.IsEquippedItem() == false)
            return;

        // To Do ..
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
