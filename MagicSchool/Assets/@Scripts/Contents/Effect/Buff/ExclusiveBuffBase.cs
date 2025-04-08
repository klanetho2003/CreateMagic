using System;
using Data;
using UnityEngine;
using static Define;

public class ExclusiveBuffBase : EffectBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        EffectType = EEffectType.ExclusiveBuff;
        return true;
    }

    public override bool ClearEffect(EEffectClearType clearType)
    {
        if (base.ClearEffect(clearType) == true)
            EffectComponent.ExclusiveActiveEffects.Remove(DataTemplateID);
        return true;
    }

    public override void SetInfo(int templateID, CreatureController owner, EEffectSpawnType spawnType, SkillBase skill)
    {
        base.SetInfo(templateID, owner, spawnType, skill);

        if (EffectData.Amount < 0 || EffectData.PercentAdd < 0)
        {
            EffectType = EEffectType.ExclusiveDeBuff;
        }
    }
}
