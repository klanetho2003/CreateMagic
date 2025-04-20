using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Define;

public class Freeze : CCBase
{
    Queue<EffectBase> _effects
    {
        get
        {
            if (EffectComponent.StackableEffects.TryGetValue(EffectData.ClassName, out Queue<EffectBase> freezes))
                return freezes;
            return null;
        }
    }

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

		EffectType = Define.EEffectType.CrowdControl;

		return true;
	}

	public override void ApplyEffect()
	{
		// Loop = false;
		base.ApplyEffect();

        AddModifier(Owner.MoveSpeed, this);
    }

    public override void ApplyStack()
    {
        base.ApplyStack();

        // 지속 시간 초기화
        if (_effects.TryPeek(out EffectBase effect))
            effect.Remains = Remains;

        Debug.Log(_effects.Count);

        if (_effects.Count == 2)
        {
            AddModifier(Owner.MoveSpeed, this);
        }
        else if (_effects.Count >= 3)
        {
            EffectComponent.GenerateEffects(EffectData.NextEffectId.ToArray(), EEffectSpawnType.Skill, Skill);
            Owner.SpriteRenderer.sprite = Owner.LastStateSprite;
        }
    }

    public override bool ClearEffect(Define.EEffectClearType clearType)
    {
        if (EffectComponent.StackableEffects[EffectData.ClassName].TryDequeue(out EffectBase self) == false)
            return false;

        RemoveModifier(Owner.MoveSpeed, self);
        Managers.Object.Despawn(self);

        return self.ClearEffect(clearType);
    }

    /*protected override void SetOwnerMaterial()
    {
        base.SetOwnerMaterial();

        Owner.SpriteRenderer.material.EnableKeyword("GLOW_ON");
        Owner.SpriteRenderer.material.EnableKeyword("FADE_ON");
        Owner.SpriteRenderer.material.EnableKeyword("OUTBASE_ON");
    }

    protected override void ResetOwnerMaterial()
    {
        if (EffectComponent.StackableEffects[EffectData.ClassName].Count > 0)
            return;

        base.ResetOwnerMaterial();

        Owner.SpriteRenderer.material.DisableKeyword("GLOW_ON");
        Owner.SpriteRenderer.material.DisableKeyword("FADE_ON");
        Owner.SpriteRenderer.material.DisableKeyword("OUTBASE_ON");
    }*/
}