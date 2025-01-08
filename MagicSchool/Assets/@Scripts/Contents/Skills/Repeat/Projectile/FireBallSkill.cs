using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSkill : SingleSkill
{
    public FireBallSkill() : base("N1QA")
    {

    }

    public override void DoSkill(Action callBack)
    {
        PlayerController pc = Managers.Game.Player;
        if (pc == null)
            return;

        Vector3 spawnPos = pc.FireSocket;
        Vector3 dir = pc.ShootDir;

        GenerateProjectile(Define.Fire_Ball_ID, Owner, spawnPos, dir, Vector3.zero);

        // To Do : 후딜레이용 코루틴
        CompleteSkillDelay(0.0f);
        callBack?.Invoke();
    }

    // ToDo : Setinfo
}
