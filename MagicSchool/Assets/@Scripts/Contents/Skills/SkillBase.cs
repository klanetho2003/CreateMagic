using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using static Define;
using static AnimationEventManager;

public abstract class SkillBase : MonoBehaviour // 스킬을 스폰 > ActiveSkill 발동 >>> 스킬 시전
{
    public CreatureController Owner { get; set; }

    public Data.SkillData SkillData { get; protected set; } // Magic Skill 전용
    public virtual void SetInfo(CreatureController owner, string skillTemplateID) // Magic Skill 전용
    {

    }

    public Data.MonsterSkillData MonsterSkillData { get; protected set; }
    public virtual void SetInfo(CreatureController owner, int monsterSkillTemplateID)
    {
        Owner = owner;
        MonsterSkillData = Managers.Data.MonsterSkillDic[monsterSkillTemplateID];

        // Handle AnimEvent
        if (Owner.Anim != null)
        {
            BindEvent(Owner, "OnAttackTarget", OnAttackTargetHandler);
            BindEvent(Owner, "OnAnimComplate", OnAnimComplateHandler);
        }
    }

    private void OnDisable() // 게임 강종
    {
        if (Managers.Game == null)
            return;
        if (Owner.IsValid() == false)
            return;
        if (Owner.Anim == null)
            return;

        UnbindEvent(Owner, "OnAttackTarget", OnAttackTargetHandler);
        UnbindEvent(Owner, "OnAnimComplate", OnAnimComplateHandler);
    }

    protected abstract void OnAttackTargetHandler();
    protected abstract void OnAnimComplateHandler();

    #region Init Method
    void Awake()
    {
        Init();
    }

    bool _init = false;

    public virtual bool Init() // 최초 실행일 떄는 true를 반환, 한 번이라도 실행한 내역이 있을 경우 false를 반환
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }
    #endregion

    public virtual void ActivateSkill()
    {

    }

    // To Do : Generate 함수 하나로 묶기
    protected virtual void GenerateProjectile(CreatureController onwer, Vector3 spawnPos, Action<CreatureController> OnHit = null)
    {
        ProjectileController projectile = Managers.Object.Spawn<ProjectileController>(spawnPos, MonsterSkillData.ProjectileId);

        // 충돌하기 싫은 친구들 settting
        LayerMask excludeMask = 0;
        excludeMask.AddLayer(ELayer.Default);
        excludeMask.AddLayer(ELayer.Projectile);
        excludeMask.AddLayer(ELayer.Env);
        excludeMask.AddLayer(ELayer.Obstacle);

        switch (Owner.CreatureType)
        {
            case ECreatureType.Student:
                excludeMask.AddLayer(ELayer.Student);
                break;
            case ECreatureType.Monster:
                excludeMask.AddLayer(ELayer.Monster);
                break;
        }

        projectile.SetSpawnInfo(Owner, this, excludeMask);
    }
    protected virtual RangeSkillController GenerateRangeSkill(Data.SkillData skillData, CreatureController onwer, float lifeTime, Vector3 spawnPos, Vector3 size, Action<CreatureController> OnHit = null)
    {
        RangeSkillController rc = Managers.Object.Spawn<RangeSkillController>(spawnPos, skillData.templateID);
        rc.SetInfo(skillData, Owner, lifeTime, size, OnHit);

        return rc;
    }



    /*public SkillBase(ESkillType skillType)
    {
        SkillType = skillType;
    }*/


    public ESkillType SkillType { get; set; } = ESkillType.None;
    

    public float ActivateDelaySecond { get; protected set; } = 0.0f;
    public float CompleteDelaySecond { get; protected set; } = 0.0f;

    public int SkillLevel { get; set; } = 0; // 탕탕이라 있는 것 -> 스킬 레벨에 따라 사용할 수 있는 스킬인지 판별할 수도 있음
    public bool IsLearnedSkill { get { return SkillLevel > 0; } }

    public  int Damage { get; protected set; } = 100; // SKillData에 들어가 있을 예정이지만, 임의 값으로 넣어줌

    

    

    

    

    #region Skill Delay
    protected Coroutine _coSkillDelay;

    //선딜
    public void ActivateSkillDelay(float waitSeconds, Action callBack)
    {
        if (waitSeconds == 0)
        {
            Owner.CreatureState = Define.CreatureState.DoSkill;
            callBack.Invoke();
            return;
        }

        if (_coSkillDelay != null)
            StopCoroutine(_coSkillDelay);

        _coSkillDelay = StartCoroutine(CoActivateSkillDelay(waitSeconds, callBack));
    }

    IEnumerator CoActivateSkillDelay(float waitSeconds, Action callBack)
    {
        yield return new WaitForSeconds(waitSeconds);

        Owner.CreatureState = Define.CreatureState.DoSkill;
        callBack.Invoke();
        _coSkillDelay = null;
    }

    //후딜
    public void CompleteSkillDelay(float waitSeconds)
    {
        if (waitSeconds == 0)
        {
            Owner.CreatureState = Define.CreatureState.Idle;
            return;
        }

        if (_coSkillDelay != null)
            StopCoroutine(_coSkillDelay);

        _coSkillDelay = StartCoroutine(CoCompleteSkillDelay(waitSeconds));
    }

    IEnumerator CoCompleteSkillDelay(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        Owner.CreatureState = Define.CreatureState.Idle;
        _coSkillDelay = null;
    }
    #endregion

    #region Destory
    Coroutine _coDestory;

    public void StartDestory<T>(T bc, float delaySeconds) where T : BaseController
    {
        if (delaySeconds < 0)
            return;

        StopDestory();

        _coDestory = StartCoroutine(CoDestroy(bc, delaySeconds));
    }

    public void StopDestory()
    {
        if (_coDestory != null)
        {
            StopCoroutine(_coDestory);
            _coDestory = null;
        }
    }

    IEnumerator CoDestroy<T>(T bc, float delaySeconds) where T : BaseController
    {
        yield return new WaitForSeconds(delaySeconds);

        if (bc.IsValid())
        {
            Managers.Object.Despawn(bc);
        }
        /*else if (this.IsValid())
        {
            Managers.Object.Despawn(this);
        }*/
    }
    #endregion
}
