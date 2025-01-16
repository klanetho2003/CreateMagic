using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingImpact : SingleSkill
{
    public CastingImpact() : base("N1")
    {
        if (Managers.Data.SkillDic.TryGetValue(Key, out Data.SkillData skillData) == false)
        {
            Debug.LogError("SingleSkill LoadData Failed");
            return;
        }

        SkillData = skillData;

        Damage = skillData.damage;

        ActivateDelaySecond = skillData.activateSkillDelay;
        CompleteDelaySecond = skillData.completeSkillDelay;
    }

    // To Do : Data Pasing
    Vector3 _defaultSize = Vector3.one;
    float _lifeTime = 0.7f;

    public override void DoSkill(Action callBack = null)
    {
        PlayerController pc = Managers.Game.Player;
        if (pc == null)
            return;

        Vector3 spawnPos = pc.transform.position;
        Vector3 dir = Vector2.zero;

        RangeSkillController rc = GenerateRangeSkill(SkillData, Owner, _lifeTime, spawnPos, _defaultSize, AfterTrigger);

        _defaultSize = _defaultSize * 1.5f;
    }

    public void InitSize()
    {
        _defaultSize = Vector3.one;
    }

    public void AfterTrigger(CreatureController cc)
    {
        if (cc.TryGetComponent<MonsterController>(out MonsterController mc))
        {
            cc.OnDamaged(Owner, Damage);
            cc.CreatureState = Define.CreatureState.Dameged; // Todo : 다시 idle이건 move건 바꿔야함
        }
        else
            return;


        /* OnKnockBack(mc, 10f);      change to       mc.MoveMonsterPosition(,,,) */
    }

    // 정해진 거리로 날아가기 + 속도를 조절 // 아래 코루틴과 함수는 삭제할 것
    /*float moveDistence = 0.0f;
    float _knockbackSpeed = 30.0f;
    Coroutine _coOnKnockBack;
    public void OnKnockBack(MonsterController mc, float distence)
    {
        if (_coOnKnockBack != null)
            StopCoroutine(_coOnKnockBack);

        _coOnKnockBack = StartCoroutine(CoOnKnockBack(mc, distence));
    }
    IEnumerator CoOnKnockBack(MonsterController mc, float distence)
    {
        Vector3 mcPosition = mc.transform.position;
        Vector3 dir = (mcPosition - Owner.transform.position).normalized;

        while (distence > moveDistence)
        {
            mc.MoveMonsterPosition(dir, _knockbackSpeed);

            moveDistence += Time.deltaTime * _knockbackSpeed;

            yield return null;
        }

        mc.CreatureState = Define.CreatureState.Moving;

        moveDistence = 0.0f;
        StopCoroutine(_coOnKnockBack);
        _coOnKnockBack = null;
    }
*/
}
