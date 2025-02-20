using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EffectComponent : MonoBehaviour
{
	public List<EffectBase> ActiveEffects = new List<EffectBase>();
	public Queue<EffectBase> BurnQueue = new Queue<EffectBase>();
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

            // Temp : Burn같은 친구들은 ElementalEffect를 상속 받는 것으로 다시 만들어야할 듯
            if (className == "Burn")
            {
                BurnQueue.Enqueue(effect);
                effect.SetInfo(id, _owner, spawnType, skill);

                if (BurnQueue.Count > 1)
                    continue;
            }
            //

            ActiveEffects.Add(effect);
            generatedEffects.Add(effect);

            effect.SetInfo(id, _owner, spawnType, skill);
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
			if (buff.EffectType != EEffectType.Buff)
			{
				buff.ClearEffect(EEffectClearType.ClearSkill);
			}
		}
	}
}
