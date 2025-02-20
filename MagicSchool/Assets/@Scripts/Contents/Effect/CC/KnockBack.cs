using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class KnockBack : CCBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        StopCoroutine((DoKnockBack()));
        StartCoroutine(DoKnockBack());
    }

    public float moveDistance { get; private set; } = 0.0f;
   
    protected IEnumerator DoKnockBack()
    {
        while (EffectData.Amount > moveDistance)
        {
            if (this.IsValid() == false)
                yield break;

            Vector3 dirNor = (Owner.transform.position - Skill.Owner.transform.position).normalized;
            Vector3 newPos = transform.position + dirNor * EFFECT_KNOCKBACK_SPEED * Time.deltaTime;

            Owner.RigidBody.MovePosition(newPos);
            Owner.SetCellPos(Managers.Map.World2Cell(newPos), false);

            moveDistance += EFFECT_KNOCKBACK_SPEED * Time.deltaTime;

            yield return null;
        }

        // 넉백 상태가 끝난 후 상태 복귀
        if (Owner.CreatureState == CreatureState.Dameged)
            Owner.CreatureState = CreatureState.Idle;

        moveDistance = 0.0f;

        ClearEffect(EEffectClearType.EndOfAirborne);
    }

    protected override IEnumerator CoStartTimer()
    {
        //Airborne는 타이머 없음
        yield break;
    }
}