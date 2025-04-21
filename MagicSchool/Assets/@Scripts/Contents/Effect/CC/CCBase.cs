using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CCBase : EffectBase
{
    protected CreatureState lastState;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

        // Already in SetIfo
		// EffectType = EEffectType.CrowdControl;

		return true;
	}

    /*protected virtual void SetRemain(EffectBase effect, ResistType resistType)
    {
        float resist = Owner.GetResistance(resistType);
        float remains = Mathf.Clamp(Remains * (1f - resist), 0, Remains);

        effect.Remains = remains;

        Debug.Log($"Effect Remains : {effect.Remains}");
    }*/

    public override void ApplyEffect()
	{
		base.ApplyEffect();

        lastState = Owner.CreatureState;

        Owner.CreatureState = CreatureState.Stun;

        // SetRemain(this, Skill.SkillData.SkillType);
    }

	public override bool ClearEffect(EEffectClearType clearType)
	{
		if (Owner.IsValid() == true && base.ClearEffect(clearType) == true)
			Owner.CreatureState = CreatureState.Idle; // lastState ?

        return true;
	}
}