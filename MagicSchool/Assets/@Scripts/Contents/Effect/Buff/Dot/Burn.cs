using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Burn : DotBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    protected override void ProcessDot()
    {
        base.ProcessDot();
    }

    public override bool ClearEffect(Define.EEffectClearType clearType)
    {
        if (EffectComponent.BurnQueue.Count < 1)
            return base.ClearEffect(clearType);

        // New Burn Start
        Burn burn = EffectComponent.BurnQueue.Dequeue() as Burn;
        EffectComponent.ActiveEffects.Add(burn);
        burn.ApplyEffect();

        return base.ClearEffect(clearType);
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
        if (EffectComponent.BurnQueue.Count > 0)
            return;

        base.ResetOwnerMaterial();

        Owner.SpriteRenderer.material.DisableKeyword("GLOW_ON");
        Owner.SpriteRenderer.material.DisableKeyword("FADE_ON");
        Owner.SpriteRenderer.material.DisableKeyword("OUTBASE_ON");
    }
}
