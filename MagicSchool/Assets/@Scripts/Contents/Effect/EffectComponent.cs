using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EffectComponent : MonoBehaviour
{
	public List<EffectBase> ActiveEffects = new List<EffectBase>();
	public Dictionary<int, EffectBase> ExclusiveActiveEffects = new Dictionary<int, EffectBase>();
	public Dictionary<string, Queue<EffectBase>> StackableEffects = new Dictionary<string, Queue<EffectBase>>();
	// public Queue<EffectBase> BurnQueue = new Queue<EffectBase>();
    private CreatureController _owner;

	public void SetInfo(CreatureController Owner)
	{
		_owner = Owner;
	}

	public List<EffectBase> GenerateEffects(IEnumerable<int> effectIds, EEffectSpawnType spawnType, SkillBase skill)
	{
		List<EffectBase> generatedEffects = new List<EffectBase>();

		foreach (int id in effectIds)
		{
			string className = Managers.Data.EffectDic[id].ClassName;
			Type effectType = Type.GetType(className);

			if (effectType == null)
			{
				Debug.LogError($"Effect Type not found: {className}");
				return null;
			}

            GameObject go = Managers.Object.SpawnGameObject(_owner.CenterPosition, "EffectBase");
            go.name = Managers.Data.EffectDic[id].ClassName;
            EffectBase effect = go.AddComponent(effectType) as EffectBase;
            effect.transform.parent = _owner.Effects.transform;
            effect.transform.localPosition = Vector2.zero;
            Managers.Object.Effects.Add(effect);
            effect.SetInfo(id, _owner, spawnType, skill);

            // Stackable Effect 처리
            if (effect.EffectType == EEffectType.StackableDebuff)
            {
                if (StackableEffects.ContainsKey(className) == false)
                {
                    Queue<EffectBase> effectQueue = new Queue<EffectBase>();
                    StackableEffects.Add(className, effectQueue);
                }

                StackableEffects[className].Enqueue(effect);
                Debug.Log($"{_owner.CreatureData.DescriptionTextID} , {StackableEffects[className].Count} testetse");
                effect.ApplyStack();

                if (StackableEffects[className].Count > 1)
                    continue;
            }

            // 중첩 불가 Effect 처리
            if (effect.EffectType == EEffectType.ExclusiveBuff || effect.EffectType == EEffectType.ExclusiveDeBuff)
            {
                // 적용 시간만 달라지는 경우 className으로 변경 필요
                if (ExclusiveActiveEffects.ContainsKey(effect.DataTemplateID))
                    continue;

                ExclusiveActiveEffects.Add(effect.DataTemplateID, effect);
            }
                
            ActiveEffects.Add(effect);
            generatedEffects.Add(effect);
            effect.ApplyEffect();
        }

		return generatedEffects;
	}

	public void RemoveEffects(EffectBase effects)
	{

	}

	public void ClearDebuffsBySkill()
	{
		foreach (var buff in ActiveEffects.ToArray())
		{
			if (buff.EffectType != EEffectType.Buff || buff.EffectType != EEffectType.ExclusiveBuff)
			{
				buff.ClearEffect(EEffectClearType.ClearSkill);
			}
		}

        foreach (var buff in ExclusiveActiveEffects.Values)
        {
            if (buff.EffectType != EEffectType.Buff || buff.EffectType != EEffectType.ExclusiveBuff)
            {
                buff.ClearEffect(EEffectClearType.ClearSkill);
            }
        }
	}

    public void Clear()
    {
        foreach (var buff in ActiveEffects.ToArray())
        {
            buff.ClearEffect(EEffectClearType.Despawn);
        }

        foreach (var buff in ExclusiveActiveEffects.Values)
        {
            buff.ClearEffect(EEffectClearType.Despawn);
        }

        ActiveEffects.Clear();
    }
}
