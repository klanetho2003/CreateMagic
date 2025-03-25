using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


[Serializable]
public class CreatureStat
{
	public float BaseValue { get; private set; }

	private bool _isChange = true; // DirtyFlag

	[SerializeField]
	private float _value;
	public virtual float Value
	{
		get
		{
			if (_isChange)
			{
				_value = CalculateFinalValue();
                _isChange = false;
			}
			return _value;
		}

		private set { _value = value; }
	}

	public List<StatModifier> StatModifiers = new List<StatModifier>();

	public CreatureStat()
	{
	}

	public CreatureStat(float baseValue) : this()
	{
		BaseValue = baseValue;
	}

	public virtual void AddModifier(StatModifier modifier)
	{
        _isChange = true;
		StatModifiers.Add(modifier);
	}

	public virtual bool RemoveModifier(StatModifier modifier)
	{
		if (StatModifiers.Remove(modifier))
		{
            _isChange = true;
			return true;
		}

		return false;
	}

	public virtual bool ClearModifiersFromSource(object source)
	{
		int numRemovals = StatModifiers.RemoveAll(mod => mod.Source == source);

		if (numRemovals > 0)
		{
            _isChange = true;
			return true;
		}
		return false;
	}

	private int CompareOrder(StatModifier a, StatModifier b)
	{
		if (a.Order == b.Order)
			return 0;

		return (a.Order < b.Order) ? -1 : 1;
	}


	private float CalculateFinalValue()
	{
		float finalValue = BaseValue;
		float sumPercentAdd = 0;

		StatModifiers.Sort(CompareOrder);

		for (int i = 0; i < StatModifiers.Count; i++)
		{
			StatModifier modifier = StatModifiers[i];

			switch (modifier.Type)
			{
                case EStatModType.None:
                    break;
				case EStatModType.Add:
					finalValue += modifier.Value;
					break;
				case EStatModType.PercentAdd:   // 합연산
					sumPercentAdd += modifier.Value;
					if (i == StatModifiers.Count - 1 || StatModifiers[i + 1].Type != EStatModType.PercentAdd)
					{
						finalValue *= 1 + sumPercentAdd;
						sumPercentAdd = 0;
					}
					break;
				case EStatModType.PercentMult:  // 곱연산
                    finalValue *= 1 + modifier.Value;
					break;
			}
		}

		return (float)Math.Round(finalValue, 4); // 4 -> 유지할 소숫점 자릿수
	}
}
