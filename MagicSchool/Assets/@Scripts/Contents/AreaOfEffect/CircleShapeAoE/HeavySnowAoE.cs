using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavySnowAoE : AoEBase
{
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
}
