using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSkill : SingleSkill
{
    public FireBallSkill() : base("N1QA")
    {
        SetData();
    }

    public override void SetData()
    {
        base.SetData();

        Damage = SkillData.damage;

        ActivateDelaySecond = SkillData.activateSkillDelay;
        CompleteDelaySecond = SkillData.completeSkillDelay;
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

    public void OnHit(CreatureController cc)
    {
        
    }
}
