using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : BaseController // 스킬을 스폰 > ActiveSkill 발동 >>> 스킬 시전
{
    public CreatureController Owner { get; set; }
    public Define.SkillType SkillType { get; set; } = Define.SkillType.None;
    public Data.SkillData SkillData { get; protected set; }

    public int SkillLevel { get; set; } = 0; // 탕탕이라 있는 것 -> 스킬 레벨에 따라 사용할 수 있는 스킬인지 판별할 수도 있음
    public bool IsLearnedSkill { get { return SkillLevel > 0; } }

    public  int Damage { get; set; } = 100; // SKillData에 들어가 있을 예정이지만, 임의 값으로 넣어줌

    public SkillBase(Define.SkillType skillType) // 상속을 받는 직속 자식들은 기본 형태의 생성자가 막히게 되며, 직속 자식은 base(skillType)을 포함하는 생성자를 만들어야 한다
    {
        SkillType = skillType;
    }

    public virtual void ActivateSkill() { }

    protected virtual void GenerateProjectile(string templateID, CreatureController onwer, Vector3 startPos, Vector3 dir, Vector3 targetPos)
    {
        ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, templateID);
        pc.SetInfo(templateID, Owner, dir);
    }

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
