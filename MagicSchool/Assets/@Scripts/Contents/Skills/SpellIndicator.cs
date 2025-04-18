using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SpellIndicator : BaseController
{
    private CreatureController _owner;
    private Data.MonsterSkillData _monsterSkillData;
    private EIndicatorType _indicateorType = EIndicatorType.Cone;

    private SpriteRenderer _coneSprite;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _coneSprite = Utils.FindChild<SpriteRenderer>(gameObject, "Cone", true);
        _coneSprite.sortingOrder = SortingLayers.SPELL_INDICATOR;

        return true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Cancel();
    }

    public void SetInfo(CreatureController creature, Data.MonsterSkillData skillData, EIndicatorType type)
    {
        _monsterSkillData = skillData;
        _indicateorType = type;
        _owner = creature;

        _coneSprite.gameObject.SetActive(false);

        _coneSprite.material.SetFloat("_Angle", 0);
        _coneSprite.material.SetFloat("_Duration", 0);
    }

    public void ShowCone(Vector3 startPos, Vector3 dir, float angleRange)
    {
        _coneSprite.gameObject.SetActive(true);
        transform.position = startPos;
        _coneSprite.material.SetFloat("_Angle", angleRange);
        _coneSprite.transform.localScale = Vector3.one * _monsterSkillData.SkillRange;
        transform.eulerAngles = GetLookAtRotation(dir);
        StartCoroutine(SetConeFill());
    }

    private IEnumerator SetConeFill()
    {
        // ������ ä���
        float elapsedTime = 0f;
        float value = 0;

        while (elapsedTime < _monsterSkillData.AnimImpactDuration)
        {
            if (_owner.CreatureState != CreatureState.DoSkill)
                Cancel();

            value = Mathf.Lerp(0f, 1f, elapsedTime / _monsterSkillData.AnimImpactDuration);
            _coneSprite.material.SetFloat("_Duration", value);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _coneSprite.gameObject.SetActive(false);
    }

    public void Cancel()
    {
        StopAllCoroutines();
        _coneSprite.gameObject.SetActive(false);
    }
}
