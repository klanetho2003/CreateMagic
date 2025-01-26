using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RepeatSkill : SkillBase
{
    public float CoolTime { get; set; } = 1.0f; // 반복 주기 _ 후에 데이터 시트로 관리 예정

    /*public RepeatSkill() : base(Define.ESkillType.Repeat)
    {

    }*/

    #region CoSkill
    Coroutine _coSkill;

    public override void ActivateSkill()
    {
        if (_coSkill != null)
        {
            StopCoroutine(_coSkill);
            _coSkill= null;
        }

        _coSkill = StartCoroutine(CoStartSkill());
    }

    protected abstract void DoSkill();

    protected virtual IEnumerator CoStartSkill()
    {
        WaitForSeconds wait = new WaitForSeconds(CoolTime);

        while (true)
        {
            DoSkill();

            yield return wait;
        }
    }
    #endregion CoSkill
}
