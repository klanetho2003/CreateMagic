using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Frozen : CCBase
{
    float _lastAnimSpeed = 0;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
    }

    protected override void SetOwnerMaterial()
    {
        base.SetOwnerMaterial();

        _lastAnimSpeed = Owner.Anim.speed;
        Owner.Anim.speed = 0;

        Owner.SpriteRenderer.material.EnableKeyword("OVERLAY_ON");
    }

    protected override void ResetOwnerMaterial()
    {
        base.ResetOwnerMaterial();

        Owner.Anim.speed = 1; // 문제 원인 > Haed Coding, Freeze의 스택 제한이 없어서 이 함수에 여러번 들어옴

        Owner.SpriteRenderer.material.DisableKeyword("OVERLAY_ON");
    }
}
