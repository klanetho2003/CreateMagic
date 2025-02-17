using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CastingImpact : SkillBase
{
    // To Do : Data Pasing
    Vector3 _defaultSize = Vector3.one;
    float _moveDistance = 10f;
    float _backSpeed = 30.0f;

    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        return true;
    }
    #endregion

    ProjectileController projectile;

    public override void SetInfo(CreatureController owner, int monsterSkillTemplateID)
    {
        base.SetInfo(owner, monsterSkillTemplateID);
    }

    public override void ActivateSkill()
    {
        if (Owner.CreatureType == ECreatureType.Monster && SkillData.AnimName != null)
        {
            Owner.Anim.Play(SkillData.AnimName, -1, 0f);
            Owner.Skills.ActivateSkills.Remove(this);

            //StartCoroutine(CoCountdownCooldown());

            projectile = GenerateProjectile(Owner, Owner.GenerateSkillPosition, ProjectileOnHit);
        }
        else if (Owner.CreatureType == ECreatureType.Student)
        {
            PlayerController pc = Owner as PlayerController;
            projectile = GenerateProjectile(Owner, pc.Shadow.position, ProjectileOnHit);
        }
    }

    public void ProjectileOnHit(BaseController cc)
    {
        if (cc.IsValid() == false)
            return;

        cc.OnDamaged(Owner, this);
    }

    protected override void OnAttackTargetHandler()
    {
        base.OnAttackTargetHandler();
    }

    protected override void OnAttackEvent()
    {
        
    }







    public void InitSize()
    {
        _defaultSize = Vector3.one;
    }

    public void AfterTrigger(BaseController bc)
    {
        if (bc.IsValid() == false)
            return;
        if (Owner.IsValid() == false)
            return;

        if (bc.ObjectType != EObjectType.Creature)
            return;

        CreatureController cc = bc.GetComponent<CreatureController>();

        cc.OnDamaged(Owner, this);

        Vector3 dir = cc.transform.position - Owner.transform.position;
        cc.MoveMonsterPosition(dir.normalized, _backSpeed, _moveDistance, () => { cc.CreatureState = Define.CreatureState.Moving; });
    }
}
