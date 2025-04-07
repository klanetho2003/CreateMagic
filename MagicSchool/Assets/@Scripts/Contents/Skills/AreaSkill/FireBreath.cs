using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBreath : AreaSkillBase
{
    public override void SetInfo(CreatureController owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);

        _angleRange = 120; // To Do Data Sheet
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
        base.OnAttackEvent();

        // Projetile Spawn���� ����
        Vector2 lookDir = (Owner.GenerateSkillPosition - Owner.CenterPosition).normalized;

        // ���⿡ ���� ����ġ ����
        Vector2 weight = Utils.ApplyPositionWeight(SkillData.RangeMultipleX, SkillData.RangeMultipleY, lookDir);

        ProjectileController _projectile = GenerateProjectile(Owner, Owner.CenterPosition + (Vector3)weight);
        _projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg);


        // To Do AreaSkillBase�� �ִ� �Լ� OnAttackEvent ���� startSkillPosition�� CenterPosition���� weight�� ���ϴ� ������ �ٲٱ�
    }
}
