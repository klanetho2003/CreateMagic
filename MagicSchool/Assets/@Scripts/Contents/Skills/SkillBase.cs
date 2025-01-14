using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : BaseController // ��ų�� ���� > ActiveSkill �ߵ� >>> ��ų ����
{
    public CreatureController Owner { get; set; }
    public Define.SkillType SkillType { get; set; } = Define.SkillType.None;
    public Data.SkillData SkillData { get; protected set; }

    public float ActivateDelaySecond { get; protected set; }
    public float CompleteDelaySecond { get; protected set; }

    public int SkillLevel { get; set; } = 0; // �����̶� �ִ� �� -> ��ų ������ ���� ����� �� �ִ� ��ų���� �Ǻ��� ���� ����
    public bool IsLearnedSkill { get { return SkillLevel > 0; } }

    public  int Damage { get; set; } = 100; // SKillData�� �� ���� ����������, ���� ������ �־���

    public SkillBase(Define.SkillType skillType) // ����� �޴� ���� �ڽĵ��� �⺻ ������ �����ڰ� ������ �Ǹ�, ���� �ڽ��� base(skillType)�� �����ϴ� �����ڸ� ������ �Ѵ�
    {
        SkillType = skillType;
    }

    public virtual void ActivateSkill() { }

    protected virtual void GenerateProjectile(Data.SkillData skillData, CreatureController onwer, Vector3 startPos, Vector3 dir, Vector3 targetPos)
    {
        ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, skillData.templateID);
        pc.SetInfo(skillData, Owner, dir);
    }

    #region Skill Delay
    protected Coroutine _coSkillDelay;

    //����
    public void ActivateSkillDelay(float waitSeconds)
    {
        if (waitSeconds == 0)
        {
            Owner.CreatureState = Define.CreatureState.DoSkill;
            return;
        }

        if (_coSkillDelay != null)
            StopCoroutine(_coSkillDelay);

        _coSkillDelay = StartCoroutine(CoActivateSkillDelay(waitSeconds));
    }

    IEnumerator CoActivateSkillDelay(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        Owner.CreatureState = Define.CreatureState.DoSkill;
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

    public void StartDestory(float delaySeconds)
    {
        StopDestory();

        _coDestory = StartCoroutine(CoDestroy(delaySeconds));
    }

    public void StopDestory()
    {
        if (_coDestory != null)
        {
            StopCoroutine(_coDestory);
            _coDestory = null;
        }
    }

    IEnumerator CoDestroy(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        if (this.IsValid())
        {
            Managers.Object.Despawn(this);
        }
    }
    #endregion
}
