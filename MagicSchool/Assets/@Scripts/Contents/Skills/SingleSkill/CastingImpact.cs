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

    public void AfterTrigger(GameObject go) //이름 수정 필요
    {
        MonsterController mc = go.GetComponent<MonsterController>();
        if (mc.IsValid() == false)
            return;

        Transform mcTransform = mc.transform;

        Vector3 dir = Owner.transform.position - mcTransform.position;
        OnKnockBack(mcTransform, dir, 10f);

        mc.OnDamaged(Owner, Damage);
    }

    // 정해진 거리로 날아가기 + 속도를 조절 // To Do : Data Pasing
    float moveDistence = 0.0f;
    float _knockbackSpeed = 10.0f;
    Coroutine _coOnKnockBack;
    public void OnKnockBack(Transform mc, Vector3 dir, float distence)
    {
        if (_coOnKnockBack != null)
            StopCoroutine(_coOnKnockBack);

        _coOnKnockBack = StartCoroutine(CoOnKnockBack(mc, dir, distence));
    }
    IEnumerator CoOnKnockBack(Transform mc, Vector3 dir, float distence)
    {
        while (distence > moveDistence)
        {
            moveDistence += Time.deltaTime * _knockbackSpeed;

            Vector3 newPos = mc.position + dir.normalized * Time.deltaTime * _knockbackSpeed;

            mc.GetComponent<Transform>().position = newPos;
            //mc.GetComponent<Rigidbody2D>().MovePosition(newPos);

            yield return null;
        }

        moveDistence = 0.0f;
        StopCoroutine(_coOnKnockBack);
        _coOnKnockBack = null;
    }
}
