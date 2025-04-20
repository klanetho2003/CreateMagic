using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DefaultSkillBase : PlayerAreaSkillBase
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
        // Casting State에서도 사용할 수 있는 Skill
        if (Owner.ObjectType != EObjectType.Student)
        {
            Debug.LogError("이 Skill은 오직 Player만 사용할 수 있도록 설계된 Skill 입니다. 개발자에게 문의하세요.");
            return;
        }

        if (Owner.Target != null)
            _skillLookDir = (Owner.Target.transform.position - Owner.transform.position).normalized;
        else
            _skillLookDir = (Owner.GenerateSkillPosition - Owner.CenterPosition).normalized;

        // 방향 + 따른 가중치 연산
        Vector2 weight = Utils.ApplyPositionWeight(SkillData.RangeMultipleX, SkillData.RangeMultipleY, _skillLookDir);
        _skillcenterPosition = Owner.CenterPosition + (Vector3)weight;

        // Input하자마자 Skill 시전
        OnAttackEvent();
    }

    protected override void OnAttackEvent()
    {
        base.OnAttackEvent();

        Projectile = null;
    }

    protected override void Clear()
    {
        base.Clear();
    }
}
