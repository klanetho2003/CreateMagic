using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Define;

public class Freeze : CCBase
{
    public List<EffectBase> NextEffects = new List<EffectBase>();

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
        float remains = firstEffect.EffectData.TickTime * firstEffect.EffectData.TickCount * (1f - resist);
        firstEffect.Remains = Mathf.Max(remains, 0);
    }

    public override void ApplyEffect()
	{
        // Loop = false;
        // base.ApplyEffect();

        ShowEffect();
        StartCoroutine(CoStartTimer());

        if (Owner.GetResistance(Skill.SkillData.SkillType) >= 1)
            return;
        AddModifier(Owner.MoveSpeed, this);
    }

    public override void ApplyStack()
    {
        base.ApplyStack();

        // 진짜 많이 더러운데, 복귀하면 깔끔하게 만들게요
        if (Owner.GetResistance(Skill.SkillData.SkillType) >= 1)
            return;

        // 지속 시간 초기화
        Freeze firstFreeze = null;
        if (_effects.TryPeek(out EffectBase firstEffect))
        {
            SetRemain(firstEffect, Skill.SkillData.SkillType);
            firstFreeze = (Freeze)firstEffect;
        }
        
        if (_effects.Count == 2)
            AddModifier(Owner.MoveSpeed, this);
        else if (_effects.Count >= 3)
        {
            if (firstFreeze.NextEffects.Count > 0)
            {
                foreach (EffectBase effect in firstFreeze.NextEffects)
                    SetRemain(effect, Skill.SkillData.SkillType);
            }
            else
            {
                if (EffectData.NextEffectId != null)
                    firstFreeze.NextEffects = Owner.Effects.GenerateEffects(EffectData.NextEffectId.ToArray(), EEffectSpawnType.Skill, Skill);
            }
        }
    }

    public override bool ClearEffect(Define.EEffectClearType clearType)
    {
        if (EffectComponent.StackableEffects[EffectData.ClassName].TryDequeue(out EffectBase self) == false)
        {
            Remains = 0; // Clear in EffectBase.ClearEffect (Remains가 0이되면서 CoStartTimer에서 처리)

            _effects.Clear();

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