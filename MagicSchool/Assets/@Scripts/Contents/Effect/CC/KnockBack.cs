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

        StopCoroutine((DoKnockBack(lastState)));
        StartCoroutine(DoKnockBack(lastState));
    }

    // TODO
    // 에어본 중에 또 에어본을 맞는 경우
    // 에어본 중에 넉백 당하는 경우
    // 넉백중에 에어본 하는 경우
    IEnumerator DoKnockBack(CreatureState lastState)
    {
        float journeyLength = EffectData.Amount;
        float totalTime = journeyLength / EFFECT_KNOCKBACK_SPEED;
        float elapsedTime = 0; // Count Time

        Vector3 originalPosition = Owner.transform.position;
        Vector3 DestPosition = (originalPosition - Skill.Owner.transform.position).normalized * journeyLength;

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = elapsedTime / totalTime;
            Vector3 currentPos = Vector3.Lerp(originalPosition, DestPosition, normalizedTime);
            Owner.transform.position = currentPos;
            Owner.SetCellPos(Managers.Map.World2Cell(currentPos), false);

            yield return null;
        }

        // 넉백 상태가 끝난 후 상태 복귀
        if (Owner.CreatureState == CreatureState.Dameged)
            Owner.CreatureState = CreatureState.Idle;

        ClearEffect(EEffectClearType.EndOfAirborne);
    }

    protected override IEnumerator CoStartTimer()
    {
        //Airborne는 타이머 없음
        yield break;
    }
}