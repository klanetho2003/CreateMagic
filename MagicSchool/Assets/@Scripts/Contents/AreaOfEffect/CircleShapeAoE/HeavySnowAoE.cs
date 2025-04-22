using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class HeavySnowAoE : AoEBase
{
    public override void SetInfo(int dataId, BaseController owner, SkillBase skill)
    {
        base.SetInfo(dataId, owner, skill);
    }

    protected override IEnumerator CoDetectTargetsPeriodically()
    {
        yield return base.CoDetectTargetsPeriodically();
    }

    protected override void DetectTargets()
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

            rangeTarget.ApplyDamage(Owner, _skill);
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
}
