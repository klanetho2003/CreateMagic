using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class MonsterController : EffectedCreature
{
    #region Animation

    public override void FlipX(bool flag) // Monster�� �»��� ������, �ݴ�� �� ��
    {
        flag = !flag;
        base.FlipX(flag);
    }

    // Wait�� Animation�� ���� �Լ� BaseController�� ����
    protected override void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case CreatureState.Idle:
                Anim.Play($"Idle");
                break;
            case CreatureState.Moving:
                Anim.Play($"Moving");
                break;
            case CreatureState.Casting:
                Anim.Play($"Casting");
                break;
            case CreatureState.FrontDelay:
                Anim.Play($"Idle");
                break;
            case CreatureState.DoSkill:
                //Anim.Play($"DoSkill");
                //Wait(1f); // Damege Animation + �ĵ����� ��� wait // To Do Animation RunTime Parsing
                break;
            case CreatureState.Dameged:
                Anim.Play($"Dameged");
                //Wait(0.75f); // Damege Animation + �ĵ����� ��� wait // To Do Animation RunTime Parsing
                break;
            case CreatureState.Dead:
                Anim.Play($"Death");
                //Wait(1.5f); // Death Animation + �ĵ����� ��� wait // To Do Animation RunTime Parsing
                break;
        }
    }

    #endregion

    #region AI - by StatePatern

    protected override void UpdateIdle()
    {
        if (Target.IsValid() == true)
            CreatureState = CreatureState.Moving;
    }

    protected override void UpdateMoving()
    {
        if (Target.IsValid() == false && LerpCellPosCompleted)
        {
            CreatureState = CreatureState.Idle;
            return;
        }

        CheckAttackTarget(AttackDistance);
    }

    protected override void UpdateDoSkill()
    {
        if (_coWait != null)
            return;
        
        if (Target.IsValid() == false && LerpCellPosCompleted/* || Target.ObjectType == EObjectType.HeroCamp*/)
        {
            CreatureState = CreatureState.Idle;
            return;
        }

        Vector3 dir = (Target.CenterPosition - CenterPosition);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = AttackDistance * AttackDistance;

        if (distToTargetSqr > attackDistanceSqr)
        {
            CreatureState = CreatureState.Moving;
            return;
        }

        // DoSkill
        SkillBase skill = Skills.CurrentSkill;
        if (skill == null)
            return;
        skill.ActivateSkillOrDelay();

        LookAtTarget(Target);

        StartWait(skill.SkillData.ActivateSkillDelay + skill.SkillData.SkillDuration);
    }

    protected override void UpdateDameged()
    {
        /*if (_coWait == null)
            CreatureState = CreatureState.Moving;*/
    }

    protected override void UpdateDead()
    {
        //if (_coWait == null) { }
            //OnDead(); To Do �ϴ� CreatureController�� �־�ξ���
    }

    void CheckAttackTarget(/*float sqrMagnitude, */float attackRange)
    {
        Vector3 dir = (Target.transform.position - transform.position);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistenceSqr = attackRange * attackRange;

        if (distToTargetSqr <= attackDistenceSqr)
            CreatureState = CreatureState.DoSkill;
        else
            CreatureState = CreatureState.Moving;
    }

    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false; // �� �� �ʱ�ȭ���� �ʵ��� �����ִ� �κ�

        CreatureType = ECreatureType.Monster;

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        Target = Managers.Object.Player;

        CreatureState = CreatureState.Idle;

        Skills = gameObject.GetOrAddComponent<BaseSkillBook>();
        Skills.SetInfo(this, CreatureData);

        AnimationEventManager.BindEvent(this, /*"OnDamaged_Complate",*/ () =>
        {
            switch (CreatureState)
            {
                case CreatureState.Idle:
                    break;
                case CreatureState.Moving:
                    break;
                case CreatureState.Casting:
                    break;
                case CreatureState.FrontDelay:
                    break;
                case CreatureState.DoSkill:
                    break;
                case CreatureState.BackDelay:
                    break;
                case CreatureState.Dameged:
                    CreatureState = CreatureState.Idle;
                    break;
                case CreatureState.Dead:
                    CreatureState = CreatureState.Idle;
                    Managers.Object.Despawn(this);
                    break;
                default:
                    break;
            }
        });
    }

    protected override void FixedUpdateMoving() // ������ ������ ������
    {
        if (CreatureState != CreatureState.Moving)
            return;
        
        if (Target.IsValid() == false)
            return;

        CheckAttackTarget(/*dir.sqrMagnitude, */AttackDistance);

        //Vector3 dir = Target.transform.position - transform.position;
        //Vector3 destPos = transform.position + (dir * MoveSpeed * Time.fixedDeltaTime * 10);
        EFindPathResult result = FindPathAndMoveToCellPos(Target.transform.position, MONSTER_DEFAULT_MOVE_DEPTH);
        LerpToCellPos(CreatureData.MoveSpeed);
    }

    #region Battle

    public override void OnDamaged(BaseController attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);
    }

    protected override void OnDead(BaseController attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);

        Clear();

        Managers.Game.KillCount++;

        //���� �� ���� ����
        JamController jc = Managers.Object.Spawn<JamController>(transform.position);

        Managers.Object.Despawn(this);
    }

    #endregion

    protected override void Clear() // To Do : �ʱ�ȭ ���� �ʿ�
    {
        StopAllCoroutines();

        #region Ư�� Coroutine�� ������� ��� �ּ� ó���� ������� �ٲ� ��
        /*if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);
        _coDotDamage = null;*/
        #endregion
    }
}
