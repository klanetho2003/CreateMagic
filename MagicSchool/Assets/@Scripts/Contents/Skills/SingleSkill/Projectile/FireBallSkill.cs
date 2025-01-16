using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSkill : SingleSkill
{
    public FireBallSkill() : base("N1QA")
    {
        if (Managers.Data.SkillDic.TryGetValue(Key, out Data.SkillData skillData) == false)
        {
            Debug.LogError("SingleSkill LoadData Failed");
            return;
        }

        SkillData = skillData;

        ActivateDelaySecond = skillData.activateSkillDelay;
        CompleteDelaySecond = skillData.completeSkillDelay;
    }

    float _lifeTime = 10f;

    public override void DoSkill(Action callBack)
    {
        PlayerController pc = Managers.Game.Player;
        if (pc == null)
            return;

        Vector3 spawnPos = pc.FireSocket;
        Vector3 dir = pc.ShootDir;

        ProjectileController projectile = GenerateProjectile(SkillData, Owner, _lifeTime, spawnPos, dir, Vector3.zero);
        projectile.StartDestory(projectile, 10f);

        callBack?.Invoke();
    }

    // ToDo : Setinfo
}
