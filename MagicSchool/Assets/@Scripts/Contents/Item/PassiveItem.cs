using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PassiveItem : Item
{
    public PassiveItem(int templateId) : base(templateId)
    {

    }
}

public class Artifact : PassiveItem
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

    //  Temp, To DO : 어떤 Stat이 어떤 StatModType 만큼 상승하는지 Class로 묶은 뒤, List로 관리해야할 듯
    public override void ApplyItemAbility(EStatModType statModType, CreatureController target)
    {
        base.ApplyItemAbility(statModType, target);

        // Inventory에 지닌 장비인가? (즉, 획득한 장비인가?)
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
