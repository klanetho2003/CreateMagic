using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HeavyStun : CCBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        EffectType = Define.EEffectType.CrowdControl;
        return true;
    }
}
