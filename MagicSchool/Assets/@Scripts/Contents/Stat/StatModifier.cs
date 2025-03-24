using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class StatModifier
{
	public readonly float Value;
	public readonly EStatModType Type;
	public readonly int Order;
	public readonly object Source;

	public StatModifier(float value, EStatModType type, int order, object source)
	{
		Value = value;
		Type = type;        // ���� Type
		Order = order;      // ���� �켱����
		Source = source;    // �߰��� ��ü
	}

    // ������
	public StatModifier(float value, EStatModType type) : this(value, type, (int)type, null) { }

	public StatModifier(float value, EStatModType type, int order) : this(value, type, order, null) { }

	public StatModifier(float value, EStatModType type, object source) : this(value, type, (int)type, source) { }
}