using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Define;

public class Freeze : CCBase
{
    Queue<EffectBase> _effects
    {
        get
        {
            if (EffectComponent.StackableEffects.TryGetValue(EffectData.ClassName, out Queue<EffectBase> freezes))
                return freezes;
            return null;
        }
    }

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

    void SetRemain(EffectBase firstEffect, ResistType resistType)
    {
        float resist = Owner.GetResistance(resistType);
        float remains = firstEffect.Remains * (1f - resist);
        firstEffect.Remains = Mathf.Clamp(remains, 0, Remains);

        // firstEffect.Remains = Remains;

        Debug.Log($"_effects.Count : {_effects.Count}");
    }

    public override void ApplyEffect()
	{
        // Loop = false;
        // base.ApplyEffect();

        ShowEffect();
        StartCoroutine(CoStartTimer());

        AddModifier(Owner.MoveSpeed, this);
    }

    public override void ApplyStack()
    {
        base.ApplyStack();

        // 지속 시간 초기화
        if (_effects.TryPeek(out EffectBase effect))
            SetRemain(effect, Skill.SkillData.SkillType); // effect.Remains = Remains;

        if (_effects.Count == 2)
        {
            AddModifier(Owner.MoveSpeed, this);
        }
        else if (_effects.Count >= 3)
        {
            if (EffectData.NextEffectId != null)
                Owner.Effects.GenerateEffects(EffectData.NextEffectId.ToArray(), EEffectSpawnType.Skill, Skill);
        }
    }

    public override bool ClearEffect(Define.EEffectClearType clearType)
    {
        if (EffectComponent.StackableEffects[EffectData.ClassName].TryDequeue(out EffectBase self) == false)
        {
            Remains = 0; // Clear in EffectBase.ClearEffect (Remains가 0이되면서 CoStartTimer에서 처리)
            return true;
        }

        RemoveModifier(Owner.MoveSpeed, self);
        EffectComponent.RemoveEffects(self, clearType);

        return self.ClearEffect(clearType);
    }

    protected override void SetOwnerMaterial()
    {
        base.SetOwnerMaterial();

        Owner.SpriteRenderer.material.EnableKeyword("OVERLAY_ON");
    }

    protected override void ResetOwnerMaterial()
    {
        // Frozen되면 이 ResetOwnerMaterial함수에 들어오지 못함 (ClearEffect하지 않고)
        // Freeze 내부에서 Managers.Object.Despawn(self);로 Effect를 Clear하기 때문

        base.ResetOwnerMaterial();

        Owner.SpriteRenderer.material.DisableKeyword("OVERLAY_ON");
    }
}