using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMotion : ProjectileMotionBase
{
    // To Do :  추가 정보 기입

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public new void SetInfo(int dataTemplateID, Vector3 startPosition, Vector3 targetPosition, Action endCallBack = null) // Add Paramiter
    {
        base.SetInfo(dataTemplateID, startPosition, targetPosition, endCallBack);
    }

    protected override IEnumerator CoLaunchProjectile()
    {
        float journeyLength = Vector3.Distance(StartPosition, TargetPosition);
        float totalTime = journeyLength / _speed;
        float elapsedTime = 0; // Count Time

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime = elapsedTime / totalTime;
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, normalizedTime);

            if (LookAtTarget)
                LookAt2D(TargetPosition - transform.position);

            yield return null;
        }

        transform.position = TargetPosition;
        EndCallback.Invoke();
    }
}
