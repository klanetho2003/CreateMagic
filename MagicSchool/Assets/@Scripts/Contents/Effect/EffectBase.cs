using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Define;

public class EffectBase : BaseController
{
    public SkillBase Skill;
    public CreatureController Owner;
    public EffectComponent EffectComponent { get { return Owner.Effects; } }
	public EffectData EffectData;
	public EEffectType EffectType;

	protected float Remains { get; set; } = 0;
	protected EEffectSpawnType _spawnType;
	protected bool Loop { get; set; } = true;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

	public virtual void SetInfo(int templateID, CreatureController owner, EEffectSpawnType spawnType, SkillBase skill)
	{
		DataTemplateID = templateID;
		EffectData = Managers.Data.EffectDic[templateID];
        Skill = skill;

        Owner = owner;
        _spawnType = spawnType;
        EffectType = EffectData.EffectType;

        if (string.IsNullOrEmpty(EffectData.SpriteID) == false)
            SpriteRenderer.sprite = Managers.Resource.Load<Sprite>(EffectData.SpriteID);
        if (string.IsNullOrEmpty(EffectData.MaterialID) == false)
            SpriteRenderer.material = Managers.Resource.Load<Material>(EffectData.MaterialID);
        if (string.IsNullOrEmpty(EffectData.AnimatorDataID) == false)
            SetAnimation(EffectData.AnimatorDataID, EffectData.SortingLayerName, SortingLayers.SKILL_EFFECT);

		// AoE
		if (_spawnType == EEffectSpawnType.External)
			Remains = float.MaxValue;
        else
            Remains = EffectData.TickTime * EffectData.TickCount;

        AnimationEventManager.BindEvent(this, () =>
        {
            // ToDo
        });
    }

	public virtual void ApplyEffect()
	{
		ShowEffect();
		StartCoroutine(CoStartTimer());
	}

	protected virtual void ShowEffect()
	{
        // Owner에게 적용한 이상상태 시각 효과 적용
        SetMaterial();

        if (Anim == null)
            return;

        if (Anim.runtimeAnimatorController == null)
            return;

        if (EffectData.AnimName != null)
            Anim.Play(EffectData.AnimName, -1, 0f);
    }

	protected void AddModifier(CreatureStat stat, object source, int order = 0)
	{
		if (EffectData.Amount != 0)
		{
			StatModifier add = new StatModifier(EffectData.Amount, EStatModType.Add, order, source);
			stat.AddModifier(add);
		}

		if (EffectData.PercentAdd != 0)
		{
			StatModifier percentAdd = new StatModifier(EffectData.PercentAdd, EStatModType.PercentAdd, order, source);
			stat.AddModifier(percentAdd);
		}

		if (EffectData.PercentMult != 0)
		{
			StatModifier percentMult = new StatModifier(EffectData.PercentMult, EStatModType.PercentMult, order, source);
			stat.AddModifier(percentMult);
		}
	}

	protected void RemoveModifier(CreatureStat stat, object source)
	{
		stat.ClearModifiersFromSource(source);
	}

	public virtual bool ClearEffect(EEffectClearType clearType)
	{
		Debug.Log($"ClearEffect - {gameObject.name} {EffectData.ClassName} -> {clearType}");

        SetMaterial(); // Owner에게 적용한 이상상태 시각 효과 초기화

        switch (clearType)
		{
			case EEffectClearType.TimeOut:
			case EEffectClearType.TriggerOutAoE:
			case EEffectClearType.EndOfAirborne:
				Managers.Object.Despawn(this);
				return true;

			case EEffectClearType.ClearSkill:
				// AoE범위 안에 있는경우 해제 X
				if (_spawnType != EEffectSpawnType.External)
				{
					Managers.Object.Despawn(this);
					return true;
				}
				break;

            case EEffectClearType.Despawn:
                Managers.Object.Despawn(this);
                return true;
        }

		return false;
	}

	protected virtual void ProcessDot()
	{

	}


	protected virtual IEnumerator CoStartTimer()
	{
		float sumTime = 0f;

		/*ProcessDot();*/ // Debuff가 걸리자마자 데미지를 줄 것인가?

		while (Remains > 0)
		{
			Remains -= Time.deltaTime;
			sumTime += Time.deltaTime;

			// 틱마다 ProcessDotTick 호출
			if (sumTime >= EffectData.TickTime)
			{
				ProcessDot();
				sumTime -= EffectData.TickTime;
			}

			yield return null;
		}

		Remains = 0;
        ClearEffect(EEffectClearType.TimeOut);
    }

    void SetMaterial()
    {
        if (Owner.IsValid() == false)
            return;
        if (Owner.SpriteRenderer.material == null)
            return;

        if (Remains > 0)
            SetOwnerMaterial();
        else
            ResetOwnerMaterial();
    }

    protected virtual void SetOwnerMaterial() { }

    protected virtual void ResetOwnerMaterial() { }
}
