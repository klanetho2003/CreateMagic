using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ActiveItem : Item
{
    #region Init
    public ActiveItem(int templateId) : base(templateId)
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
    #endregion
}

#region Equipment
public class Equipment : ActiveItem
{
    #region Init
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

        /*if (TemplateData.Type != EItemType.Weapon && TemplateData.Type != EItemType.Armor)
            return false;*/

        // Equipment Data Parsing
        /*ArtifactData data = (ArtifactData)TemplateData;
        {
            Damage = data.Damage;
            Defence = data.Defence;
            Speed = data.Speed;
        }*/

        return true;
    }
    #endregion

    public override void ApplyItemAbility(EStatModType statModType, CreatureController target)
    {
        base.ApplyItemAbility(statModType, target);

        // ������ ����ΰ�?
        if (this.IsEquippedItem() == false)
            return;

        // To Do ..
    }

    public override void UseItem(CreatureController target)
    {
        base.UseItem(target);

        // ������ ����ΰ�?
        if (this.IsEquippedItem() == false)
            return;

        // To Do ..
    }
}

public class ItemSkill : Equipment
{
    public ItemSkill(int templateId) : base(templateId)
    {
    }
}
#endregion

#region Consumable
public class Consumable : ActiveItem
{
    public float Value { get; private set; }

    protected Data.ConsumableData ConsumableData { get { return (Data.ConsumableData)TemplateData; } }

    public Consumable(int templateId) : base(templateId)
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (TemplateData.ItemGroupType != EItemGroupType.Consumable)
            return false;

        ConsumableData data = (ConsumableData)TemplateData;
        {
            Value = data.Value;
        }

        return true;
    }
    public override void UseItem(CreatureController target)
    {
        base.UseItem(target);

        // ������ ����ΰ�?
        if (this.IsEquippedItem() == false)
            return;

        // To Do ..
    }

}

public class Potion : Consumable
{
    // �߰��� Value ����

    public Potion(int templateId) : base(templateId)
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (TemplateData.Type != EItemType.Potion) // Potion�� ItemType�� Hp�� Mp�� �б��ؼ� ������ ��(�翬�� Potion type�� ����)
            return false;

        // Value Parsing

        return true;
    }

    public override void UseItem(CreatureController target)
    {
        base.UseItem(target);

        // ���� ���
    }
}

public class Scroll : Consumable
{
    public Scroll(int templateId) : base(templateId)
    {
    }
}
#endregion