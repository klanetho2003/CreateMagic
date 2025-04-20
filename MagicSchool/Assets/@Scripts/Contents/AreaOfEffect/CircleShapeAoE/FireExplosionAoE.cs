using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class FireExplosionAoE : AoEBase
{
    #region Init & Disable
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
    #endregion

    public override void SetInfo(int dataId, BaseController owner, SkillBase skill)
    {
        base.SetInfo(dataId, owner, skill);
    }

    protected override IEnumerator CoDetectTargetsPeriodically()
    {
        yield return base.CoDetectTargetsPeriodically();
    }

    protected override void DetectTargets()
    {
        base.DetectTargets();
    }

    // To Pooling
    protected override IEnumerator CoReserveDestroy()
    {
        yield return new WaitForSeconds(_aoEData.Duration);
        Managers.Object.Despawn(this);
    }
}
