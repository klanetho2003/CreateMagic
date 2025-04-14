using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ActiveItem : Item
{
    public ActiveItem(int templateId) : base(templateId)
    {

    }
}

public class Equipment : ActiveItem
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

        // 장착된 장비인가?
        if (this.IsEquippedItem() == false)
            return;

        // To Do ..
    }
}