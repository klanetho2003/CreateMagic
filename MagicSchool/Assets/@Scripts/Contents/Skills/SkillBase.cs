using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : BaseController // ��ų�� ���� > ActiveSkill �ߵ� >>> ��ų ����
{
    public CreatureController Owner { get; set; }
    public Define.SkillType SkillType { get; set; } = Define.SkillType.None;
    public Data.SkillData SkillData { get; protected set; }

    public float ActivateDelaySecond { get; protected set; } = 0.0f;
    public float CompleteDelaySecond { get; protected set; } = 0.0f;

    public int SkillLevel { get; set; } = 0; // �����̶� �ִ� �� -> ��ų ������ ���� ����� �� �ִ� ��ų���� �Ǻ��� ���� ����
    public bool IsLearnedSkill { get { return SkillLevel > 0; } }

    public  int Damage { get; protected set; } = 100; // SKillData�� �� ���� ����������, ���� ������ �־���

    public SkillBase(Define.SkillType skillType) // ����� �޴� ���� �ڽĵ��� �⺻ ������ �����ڰ� ������ �Ǹ�, ���� �ڽ��� base(skillType)�� �����ϴ� �����ڸ� ������ �Ѵ�
    {
        SkillType = skillType;
    }

    public virtual void ActivateSkill() { }

    // Init�Լ� : spawn ���� ���� �����ϱ� ���� �ؾ��� �ൿ ����
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

    //����
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

    //�ĵ�
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
