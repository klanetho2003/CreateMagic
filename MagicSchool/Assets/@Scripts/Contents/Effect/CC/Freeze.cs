using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : CCBase
{
    Queue<EffectBase> _effects { get { return EffectComponent.StackableEffects[EffectData.ClassName]; } }

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

        Owner.SpriteRenderer.sprite = Owner.LastStateSprite;
    }

    public override void ApplyStack()
    {
        base.ApplyStack();

        // 지속 시간 초기화
        if (_effects.TryPeek(out EffectBase effect))
            effect.Remains = Remains;

        // 1스택
        // 2스택
        // 3스택
    }

    public override bool ClearEffect(Define.EEffectClearType clearType)
    {
        base.ClearEffect(clearType);

        _effects.Clear();

        return true;
    }


    protected override void SetOwnerMaterial()
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
    }
}