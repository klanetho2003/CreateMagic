using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleSkill : SkillBase
{
    public SingleSkill(string key) : base(Define.SkillType.Single)
    {
        this.key = key;
    }

    public string key { get; protected set; }

    public abstract void DoSkill(Action callBack = null);

    public override void ActivateSkill()
    {
        DoSkill(OnFinishedSingleSkill);
    }

    void OnFinishedSingleSkill()
    {
        Owner.CreatureState = Define.CreatureState.Idle;
    }

    #region Skill Delay
    Coroutine _coSkillDelay;

    //¼±µô
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

    //ÈÄµô
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
}
