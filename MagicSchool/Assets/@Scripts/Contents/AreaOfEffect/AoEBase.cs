using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class AoEBase : BaseController
{
	[SerializeField]
	protected List<EffectBase> _activeEffects = new List<EffectBase>();

	public CreatureController Owner;
	protected HashSet<CreatureController> _targets = new HashSet<CreatureController>();
	protected SkillBase _skill;
	protected AoEData _aoEData;
	protected Vector3 _skillDir;
	protected float _radius;

	private CircleCollider2D _collider;
	private EEffectSize _effectSize;

	protected override void OnDisable()
	{
		base.OnDisable();

        // Debuff는 AoE가 삭제 되어도 유지 되어야하지 않을까

		//// 1. clear target
		//_targets.Clear();
        //
		//// 2. clear Effect
		//foreach (var effect in _activeEffects)
		//{
		//	if (effect.IsValid())
		//		effect.ClearEffect(EEffectClearType.TriggerOutAoE);
		//}
		//_activeEffects.Clear();
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		_collider = GetComponent<CircleCollider2D>();
		_collider.isTrigger = true;

		return true;
	}

	public virtual void SetInfo(int dataId, BaseController owner, SkillBase skill)
	{
		transform.localEulerAngles = Vector3.zero;
		_aoEData = Managers.Data.AoEDic[dataId];
		Owner = owner as CreatureController;
		_skill = skill;
		_effectSize = skill.SkillData.EffectSize;
		_radius = Utils.GetEffectRadius(_effectSize);
		_collider.radius = _radius;
        if (Owner.Target.IsValid() == true)
            _skillDir = (Owner.Target.transform.position - Owner.transform.position).normalized;

        SetAnimation(_aoEData.AnimatorDataID, _aoEData.SortingLayerName, SortingLayers.SKILL_EFFECT);

        StartCoroutine(CoReserveDestroy());
        StartCoroutine(CoDetectTargetsPeriodically());
    }

    protected virtual IEnumerator CoDetectTargetsPeriodically()
    {
        while (true)
        {
            DetectTargets();

            yield return new WaitForSeconds(_aoEData.TickTime); // To Do 적용 주기 파싱
        }
    }

    protected virtual void DetectTargets()
    {
        List<CreatureController> rangeTargets = Managers.Object.FindCircleRangeTargets(Owner, transform.position, _radius);
        List<CreatureController> removeTargets = new List<CreatureController>();

        foreach (CreatureController rangeTarget in rangeTargets)
        {
            if (rangeTarget.IsValid() == false)
            {
                _targets.Remove(rangeTarget);
                continue;
            }

            //_targets에 없으면 추가
            if (_targets.Contains(rangeTarget) == false)
                _targets.Add(rangeTarget);

            // rangeTarget.SumHp(Owner, _skill);
            List<EffectBase> effects = rangeTarget.Effects.GenerateEffects(_aoEData.EnemyEffects.ToArray(), EEffectSpawnType.Skill, _skill);
            if (effects.Count != 0) _activeEffects.AddRange(effects);
        }

        foreach (CreatureController target in _targets)
        {
            if (target.IsValid() == false || rangeTargets.Contains(target) == false)
                removeTargets.Add(target);
        }

        foreach (var removeTarget in removeTargets)
        {
            // 범위 밖으로 나간 Creature 처리
            // RemoveEffect(removeTarget); // AoE범위 밖으로 나갔을 때 즉시 Effect Remove
            _targets.Remove(removeTarget);
            
        }
    }

    private void RemoveEffect(CreatureController target)
    {
        List<EffectBase> effectsToRemove = new List<EffectBase>();

        foreach (var effect in _activeEffects)
        {
            if (target.Effects.ActiveEffects.Contains(effect))
            {
                effect.ClearEffect(EEffectClearType.TriggerOutAoE);
                effectsToRemove.Add(effect);
            }
        }

        foreach (var effect in effectsToRemove)
        {
            _activeEffects.Remove(effect);
        }
    }

    protected virtual IEnumerator CoReserveDestroy()
    {
        yield return new WaitForSeconds(_aoEData.Duration);
        DestroyAoE();
    }

    protected void DestroyAoE()
    {
        Managers.Object.Despawn(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        float radius = Utils.GetEffectRadius(_skill.SkillData.EffectSize);
        Gizmos.DrawWireSphere(CenterPosition, radius);
    }

    #region 현재 사용X _ 혹시 아군 적군 각각 다른 효과를 부여하고 싶을 때 참고해서 수정보완
    protected void ApplyEffectsInRange(int angle)
	{
		// 아군에게 버프 적용
		var allies = FindTargets(angle, true);
		ApplyEffectsToTargets(allies, _aoEData.AllyEffects.ToArray(), false);

		// 적군에게 버프 적용
		var enemies = FindTargets(angle, false);
		ApplyEffectsToTargets(enemies, _aoEData.EnemyEffects.ToArray(), true);
	}

	private List<CreatureController> FindTargets(int angle, bool isAlly)
	{
		if (angle == 360)
			return Managers.Object.FindCircleRangeTargets(Owner, Owner.transform.position, _radius, isAlly);
		else
			return Managers.Object.FindConeRangeTargets(Owner, Owner.transform.position, _skillDir, _radius, angle, isAlly);
	}

	private void ApplyEffectsToTargets(List<CreatureController> targets, int[] effects, bool applyDamage)
	{
        if (targets.Count == 0)
            return;

		foreach (var target in targets)
		{
            CreatureController t = target as CreatureController;
			if (t.IsValid() == false)
				continue;

			t.Effects.GenerateEffects(effects, EEffectSpawnType.Skill, _skill);

			if (applyDamage)
				t.OnDamaged(Owner, _skill);
		}
	}
    #endregion
}
