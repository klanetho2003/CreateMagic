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
    // ��� �߿� �� ����� �´� ���
    // ��� �߿� �˹� ���ϴ� ���
    // �˹��߿� ��� �ϴ� ���
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

        // �˹� ���°� ���� �� ���� ����
        if (Owner.CreatureState == CreatureState.Dameged)
            Owner.CreatureState = CreatureState.Idle;

        ClearEffect(EEffectClearType.EndOfAirborne);
    }

    protected override IEnumerator CoStartTimer()
    {
        //Airborne�� Ÿ�̸� ����
        yield break;
    }
}