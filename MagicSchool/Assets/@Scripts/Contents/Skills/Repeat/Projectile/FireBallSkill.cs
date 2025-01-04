using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSkill : RepeatSkill
{
    public FireBallSkill()
    {

    }

    protected override void DoSkillJob()
    {
        if (Managers.Game.Player == null)
            return;

        Vector3 spawnPos = Managers.Game.Player.FireSocket;
        Vector3 dir = Managers.Game.Player.ShootDir;

        GenerateProjectile((int)Define.SkillID.Fire_Ball_ID, Owner, spawnPos, dir, Vector3.zero);
    }

    // ToDo : Setinfo
}
