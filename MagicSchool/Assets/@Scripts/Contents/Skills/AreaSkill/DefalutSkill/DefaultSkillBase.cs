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
        // Casting State������ ����� �� �ִ� Skill
        if (Owner.ObjectType != EObjectType.Student)
        {
            Debug.LogError("�� Skill�� ���� Player�� ����� �� �ֵ��� ����� Skill �Դϴ�. �����ڿ��� �����ϼ���.");
            return;
        }

        if (Owner.Target != null)
            _skillLookDir = (Owner.Target.transform.position - Owner.transform.position).normalized;
        else
            _skillLookDir = (Owner.GenerateSkillPosition - Owner.CenterPosition).normalized;

        // ���� + ���� ����ġ ����
        Vector2 weight = Utils.ApplyPositionWeight(SkillData.RangeMultipleX, SkillData.RangeMultipleY, _skillLookDir);
        _skillcenterPosition = Owner.CenterPosition + (Vector3)weight;

        // Input���ڸ��� Skill ����
        OnAttackEvent();
    }

    protected override void OnAttackEvent()
    {
        base.OnAttackEvent();
    }

    protected override void Clear()
    {
        base.Clear();
    }
}
