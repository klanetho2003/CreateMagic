using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : BaseController // 스킬을 스폰 > ActiveSkill 발동 >>> 스킬 시전
{
    public CreatureController Owner { get; set; }
    public Define.SkillType SkillType { get; set; } = Define.SkillType.None;
    public Data.SkillData SkillData { get; protected set; }

    public float ActivateDelaySecond { get; protected set; } = 0.0f;
    public float CompleteDelaySecond { get; protected set; } = 0.0f;

    public int SkillLevel { get; set; } = 0; // 탕탕이라 있는 것 -> 스킬 레벨에 따라 사용할 수 있는 스킬인지 판별할 수도 있음
    public bool IsLearnedSkill { get { return SkillLevel > 0; } }

    public  int Damage { get; protected set; } = 100; // SKillData에 들어가 있을 예정이지만, 임의 값으로 넣어줌

    public SkillBase(Define.SkillType skillType) // 상속을 받는 직속 자식들은 기본 형태의 생성자가 막히게 되며, 직속 자식은 base(skillType)을 포함하는 생성자를 만들어야 한다
    {
        SkillType = skillType;
    }

    public virtual void ActivateSkill() { }

    // Init함수 : spawn 직후 값을 세팅하기 전에 해야할 행동 정의
    protected virtual ProjectileController GenerateProjectile(Data.SkillData skillData, CreatureController onwer, float lifeTime, Vector3 startPos, Vector3 dir, Vector3 targetPos)
    {
        ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, skillData.templateID);
        pc.SetInfo(skillData, Owner,lifeTime, dir);

        return pc;
    }
    protected virtual RangeSkillController GenerateRangeSkill(Data.SkillData skillData, CreatureController onwer, float lifeTime, Vector3 spawnPos, Vector3 size, Action<GameObject> afterTrigger = null)
    {
        RangeSkillController rc = Managers.Object.Spawn<RangeSkillController>(spawnPos, skillData.templateID);
        rc.SetInfo(skillData, Owner, lifeTime, size, afterTrigger);

        return rc;
    }

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
        else if (this.IsValid())
        {
            Managers.Object.Despawn(this);
        }
    }
    #endregion
}
