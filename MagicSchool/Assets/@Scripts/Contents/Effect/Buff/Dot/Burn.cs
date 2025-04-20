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

    public override void ApplyStack()
    {
        base.ApplyStack();
    }

    public override bool ClearEffect(Define.EEffectClearType clearType)
    {
        // 적용되고 있는 '나' Queue에서 제거
        if (EffectComponent.StackableEffects[EffectData.ClassName].TryDequeue(out EffectBase self) == false)
            return false;

        // Despawn 시 재귀하면서 완전 제거 // CleatSkill일 시도 같은 방법으로 진핼할지 생각
        if (clearType == Define.EEffectClearType.Despawn)
            return self.ClearEffect(clearType);

        // 나 말고도 Burn이 더 존재한다면
        if (EffectComponent.StackableEffects[EffectData.ClassName].TryPeek(out EffectBase burn))
        {
            EffectComponent.ActiveEffects.Add(burn);
            burn.ApplyEffect();
        }

        return base.ClearEffect(clearType);
    }

    #region Set Material
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
    #endregion
}
