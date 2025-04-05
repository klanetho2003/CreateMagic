using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    public Data.MonsterData MonsterData { get { return (Data.MonsterData)CreatureData; } }

    #region Animation

    public override void FlipX(bool flag) // Monster는 태생이 오른쪽, 반대로 값 넣
    {
        flag = !flag;
        base.FlipX(flag);
    }

    // Wait와 Animation을 묶는 함수 BaseController에 넣자
    protected override void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case CreatureState.Spawning:
                Anim.Play($"Spawning");
                break;
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
                //Wait(1f); // Damege Animation + 후딜레이 재생 wait // To Do Animation RunTime Parsing
                break;
            case CreatureState.Dameged:
                Anim.Play($"Dameged");
                //Wait(0.75f); // Damege Animation + 후딜레이 재생 wait // To Do Animation RunTime Parsing
                break;
            case CreatureState.Dead:
                Anim.Play($"Death");
                //Wait(1.5f); // Death Animation + 후딜레이 재생 wait // To Do Animation RunTime Parsing
                break;
        }
    }

    #endregion

    #region AI - by StatePatern

    protected override void UpdateSpawning()
    {
        if (Target.IsValid() == false)
            return;
        if (_coWait != null)
            return;

        CreatureState = CreatureState.Idle;
    }

    protected override void UpdateIdle()
    {
        if (Target.IsValid() == false)
            return;

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

            //OnDead(); To Do 일단 CreatureController에 넣어두었다
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
            return false; // 두 번 초기화하지 않도록 끊어주는 부분

        ObjectType = EObjectType.Monster;

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        Target = Managers.Object.Player;

        CreatureState = CreatureState.Spawning;
        StartWait(CreatureData.SpawnDelaySeconds);

        if (Skills != null) // Remove Component when Pooling revive
        {
            foreach (SkillBase skill in Skills.SkillList)
                Destroy(skill);
        }

        Skills = gameObject.GetOrAddComponent<BaseSkillBook>();
        Skills.SetInfo(this, MonsterData);

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

    protected override void FixedUpdateMoving() // 물리와 연관돼 있으면
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

        // To Do : Remove Drop Item
        int dropItemId = MonsterData.DropItemId;

        RewardData rewardData = GetRandomReward();
        if (rewardData != null)
        {
            var itemHolder = Managers.Object.Spawn<ItemHolder>(transform.position, dropItemId);
            Vector2 ran = new Vector2(transform.position.x + UnityEngine.Random.Range(-10, -15) * 0.1f, transform.position.y);
            Vector2 ran2 = new Vector2(transform.position.x + UnityEngine.Random.Range(10, 15) * 0.1f, transform.position.y);
            Vector2 dropPos = UnityEngine.Random.value < 0.5 ? ran : ran2;
            itemHolder.SetInfo(0, rewardData.ItemTemplateId, dropPos);
        }

        Clear();

        // Broadcast
        Managers.Game.KillCount++;
        Managers.Game.BroadcastEvent(EBroadcastEventType.KillMonster, MonsterData.DataId);
    }

    #endregion

    RewardData GetRandomReward()
    {
        if (MonsterData == null)
            return null;

        if (Managers.Data.DropTableDic.TryGetValue(MonsterData.DropItemId, out DropTableData dropTableData) == false)
            return null;

        if (dropTableData.Rewards.Count <= 0)
            return null;

        int sum = 0;
        int randValue = UnityEngine.Random.Range(0, 100);

        foreach (RewardData item in dropTableData.Rewards)
        {
            sum += item.Probability;

            if (randValue <= sum)
                return item;
        }

        //return dropTableData.Rewards.RandomElementByWeight(e => e.Probability);
        return null;
    }

    int GetRewardExp()
    {
        if (MonsterData == null)
            return 0;

        if (Managers.Data.DropTableDic.TryGetValue(MonsterData.DropItemId, out DropTableData dropTableData) == false)
            return 0;

        return dropTableData.RewardExp;
    }

    protected override void Clear() // To Do : 초기화 내용 필요
    {
        StopAllCoroutines();

        Effects.Clear();
        #region 특정 Coroutine만 멈춰야할 경우 주석 처리한 방법으로 바꿀 것
        /*if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);
        _coDotDamage = null;*/
        #endregion
    }
}
