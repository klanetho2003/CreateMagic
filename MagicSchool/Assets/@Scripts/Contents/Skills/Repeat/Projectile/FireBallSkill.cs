using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSkill : SingleSkill
{
    public FireBallSkill() : base("N1QA")
    {

    }

    public override void ActivateSkill()
    {
        if (Managers.Game.Player == null)
            return;

        Vector3 spawnPos = Managers.Game.Player.FireSocket;
        Vector3 dir = Managers.Game.Player.ShootDir;

        GenerateProjectile(Define.Fire_Ball_ID, Owner, spawnPos, dir, Vector3.zero);
    }

    // ToDo : Setinfo
}
