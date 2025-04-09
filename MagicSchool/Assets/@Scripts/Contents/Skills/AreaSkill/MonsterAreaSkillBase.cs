using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Monster 전용, Player는 PlayerAreaSkillBase를 사용할 것
public class MonsterAreaSkillBase : MonsterSkillBase
{
    protected SpellIndicator _indicator;
    protected Define.EIndicatorType _indicatorType = Define.EIndicatorType.Cone;
    protected int _angleRange = 360;

    public ProjectileController Projectile { get; protected set; }
    protected Vector2 _skillLookDir { get; set; }
    protected Vector3 _skillcenterPosition { get; set; }

    public override void SetInfo(CreatureController owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();

        if (Owner.CreatureState != Define.CreatureState.DoSkill)
            return;

        if (Owner.Target != null)
            _skillLookDir = (Owner.Target.transform.position - Owner.transform.position).normalized;
        else
            _skillLookDir = (Owner.GenerateSkillPosition - Owner.CenterPosition).normalized;

        // 방향 + 따른 가중치 연산
        Vector2 weight = Utils.ApplyPositionWeight(SkillData.RangeMultipleX, SkillData.RangeMultipleY, _skillLookDir);
        _skillcenterPosition = Owner.CenterPosition + (Vector3)weight;
    }

    public override void CancelSkill()
    {
        if (_indicator)
            _indicator.Cancel();
    }

    protected void AddIndicatorComponent()
    {
        _indicator = Utils.FindChild<SpellIndicator>(gameObject, recursive: true);

        if (_indicator == null)
        {
            GameObject go = Managers.Resource.Instantiate(SkillData.PrefabLabel, gameObject.transform);
            _indicator = go.GetOrAddComponent<SpellIndicator>();
        }
    }

    protected void SpawnSpellIndicator()
    {
        /*if (Owner.Target.IsValid() == false)
            return;*/

        _indicator.ShowCone(Owner.transform.position, _skillLookDir.normalized, _angleRange);
    }

    protected override void OnAttackTargetHandler()
    {
        base.OnAttackTargetHandler();
    }

    protected override void OnAttackEvent()
    {
        // Projectile
        if (SkillData.ProjectileId != 0)
            Projectile = GenerateProjectile(Owner, _skillcenterPosition);

        // Damage 판정 범위 연산
        float radius = Utils.GetEffectRadius(SkillData.EffectSize);
        List<CreatureController> targets = Managers.Object.FindConeRangeTargets(Owner, _skillcenterPosition, _skillLookDir, radius, _angleRange);

        foreach (var target in targets)
        {
            if (target.IsValid())
            {
                target.OnDamaged(Owner, this);
            }
        }
    }

    protected override IEnumerator CoCountdownCooldown()
    {
        return base.CoCountdownCooldown();
    }
}
