using System.Collections;
using System.Collections.Generic;
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

    protected override void SetOwnerMaterial()
    {
        base.SetOwnerMaterial();

        Owner.SpriteRenderer.material.EnableKeyword("GLOW_ON");
        Owner.SpriteRenderer.material.EnableKeyword("FADE_ON");
        Owner.SpriteRenderer.material.EnableKeyword("OUTBASE_ON");
    }

    protected override void ResetOwnerMaterial()
    {
        base.ResetOwnerMaterial();

        Owner.SpriteRenderer.material.DisableKeyword("GLOW_ON");
        Owner.SpriteRenderer.material.DisableKeyword("FADE_ON");
        Owner.SpriteRenderer.material.DisableKeyword("OUTBASE_ON");
    }
}
