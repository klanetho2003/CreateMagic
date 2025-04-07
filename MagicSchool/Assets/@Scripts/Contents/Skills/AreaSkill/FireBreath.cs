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

        // Projetile Spawn으로 묶자
        Vector2 lookDir = (Owner.GenerateSkillPosition - Owner.CenterPosition).normalized;

        // 방향에 따른 가중치 연산
        Vector2 weight = Utils.ApplyPositionWeight(SkillData.RangeMultipleX, SkillData.RangeMultipleY, lookDir);

        ProjectileController _projectile = GenerateProjectile(Owner, Owner.CenterPosition + (Vector3)weight);
        _projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg);


        // To Do AreaSkillBase에 있는 함수 OnAttackEvent 내에 startSkillPosition을 CenterPosition에서 weight를 셈하는 것으로 바꾸기
    }
}
