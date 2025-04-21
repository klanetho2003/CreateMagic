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

        return true;
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        lastState = Owner.CreatureState;

		Owner.CreatureState = CreatureState.Stun;
    }
}
