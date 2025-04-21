using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavySnow : PlayerSkillBase
{
    #region Init Method
    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        return true;
    }
    #endregion

    public override void SetInfo(CreatureController owner, int monsterSkillTemplateID)
    {
        base.SetInfo(owner, monsterSkillTemplateID);
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();
    }

    protected override void OnAttackTargetHandler()
    {
        base.OnAttackTargetHandler();
    }

    protected override void OnAttackEvent()
    {
        Vector2 _skillLookDir = Vector2.zero;

        if (Owner.Target != null)
            _skillLookDir = (Owner.Target.transform.position - Owner.transform.position).normalized;
        else
            _skillLookDir = (Owner.GenerateSkillPosition - Owner.CenterPosition).normalized;

        // 방향 + 따른 가중치 연산
        Vector2 weight = Utils.ApplyPositionWeight(SkillData.RangeMultipleX, SkillData.RangeMultipleY, _skillLookDir);
        Vector3 aoeGeneratePosition = Owner.CenterPosition + (Vector3)weight;

        GenerateAoE(aoeGeneratePosition);
    }

    protected override void Clear()
    {
        base.Clear();
    }
}
