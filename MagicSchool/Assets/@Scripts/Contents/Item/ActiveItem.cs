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

        // 장착된 장비인가?
        if (this.IsEquippedItem() == false)
            return;

        // To Do ..
    }

    public void UseItemSkill()
    {
        // To Do
    }
}

public class ItemSkill : Equipment
{
    public ItemSkill(int templateId) : base(templateId)
    {
    }
}

public class Consumable : ActiveItem
{
    public float Value { get; private set; }

    public Consumable(int templateId) : base(templateId)
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ConsumableData data = (ConsumableData)TemplateData;
        {
            Value = data.Value;
        }

        return true;
    }
}

public class Potion : Consumable
{
    public Potion(int templateId) : base(templateId)
    {
    }
}

public class Scroll : Consumable
{
    public Scroll(int templateId) : base(templateId)
    {
    }
}