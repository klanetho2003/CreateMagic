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
        
        if (Managers.Data.SkillDic.TryGetValue(SkillData.skills[0], out Explosion) == false)
        {
            Debug.LogError("FireBallSkill InerSkill LoadData Failed");
            return;
        }

        Damage = SkillData.damage;

        ActivateDelaySecond = SkillData.activateSkillDelay;
        CompleteDelaySecond = SkillData.completeSkillDelay;
    }

    Data.SkillData Explosion;
    ProjectileController projectile;

    float _lifeTime = 1f; // To Do : Data Parsing

    public override void DoSkill(Action callBack)
    {
        PlayerController pc = Managers.Game.Player;
        if (pc == null)
            return;

        Vector3 spawnPos = pc.FireSocket;
        Vector3 dir = pc.ShootDir;

        projectile = GenerateProjectile(SkillData, Owner, _lifeTime, spawnPos, dir, Vector3.zero, ProjectileOnHit);
        projectile.StartDestory(projectile, 10f);

        callBack?.Invoke();
    }

    public void ProjectileOnHit(CreatureController cc)
    {
        if (cc.IsValid() == false)
            return;

        cc.OnDamaged(Owner, SkillData.damage);
        GenerateRangeSkill(Explosion, Owner, _lifeTime, projectile.transform.position, Vector2.one, ExplosionOnHit);
    }

    public void ExplosionOnHit(CreatureController cc)
    {
        if (cc.IsValid() == false)
            return;

        cc.OnDamaged(Owner, Explosion.damage);
        
        cc.OnBurnEx(Owner, 3); // Extension // To Do : Parsing
    }
}
