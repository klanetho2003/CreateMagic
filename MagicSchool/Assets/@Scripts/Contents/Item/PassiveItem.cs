using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PassiveItem : Item
{
    #region Init
    public PassiveItem(int templateId) : base(templateId)
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

    public override void ApplyItemAbility(EStatModType statModType, CreatureController target)
    {
        base.ApplyItemAbility(statModType, target);

        // Inventory에 지닌 장비인가? (즉, 획득한 장비인가?)
        if (this.IsInInventory() == false)
            return;
    }

    public override void RemoveItemAbility(CreatureController target)
    {
        base.RemoveItemAbility(target);

        // Inventory에 지닌 장비인가? (즉, 획득한 장비인가?)
        if (this.IsInInventory() == false)
            return;
    }
}

// StatBoost Item
public class StatBoost : PassiveItem
{
    public List<StatModifier> statModifiers { get; } = new List<StatModifier>();
    public float Damage { get; private set; }
    public float Defence { get; private set; }
    public double Speed { get; private set; }

    protected Data.StatBoostData StatBoostData { get { return (Data.StatBoostData)TemplateData; } }

    #region Init
    public StatBoost(int templateId) : base(templateId)
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (TemplateData.Type != EItemType.StatBoost)
            return false;

        StatBoostData data = (StatBoostData)TemplateData;
        {
            Damage = data.Damage;
            Defence = data.Defence;
            Speed = data.Speed;
        }

        return true;
    }
    #endregion

    StatModifier MakeModifier(float amount, EStatModType statModType)
    {
        StatModifier statMod = new StatModifier(this.Damage, statModType);
        statModifiers.Add(statMod);
        return statMod;
    }

    //  Temp, To DO : 어떤 Stat이 어떤 StatModType 만큼 상승하는지 Class로 묶은 뒤, List로 관리해야할 듯
    public override void ApplyItemAbility(EStatModType statModType, CreatureController owner)
    {
        base.ApplyItemAbility(statModType, owner);

        if (this.IsHaveAtkWeight())
        {
            StatModifier atk = MakeModifier(this.Damage, statModType);
            owner.Atk.AddModifier(atk);

            Debug.Log($"Atk : {owner.Atk.Value}");
        }

        if (this.IsHaveDefWeight())
        {
            StatModifier def = MakeModifier(this.Defence, statModType);
            owner.Atk.AddModifier(def);
        }

        if (this.IsHaveSpeedWeight())
        {
            StatModifier speed = MakeModifier((float)this.Speed, statModType);
            owner.MoveSpeed.AddModifier(speed);
        }
    }

    public override void RemoveItemAbility(CreatureController target)
    {
        base.RemoveItemAbility(target);

        foreach (var modifier in statModifiers)
        {
            // 제거됐으면 true 불가하면 false
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
