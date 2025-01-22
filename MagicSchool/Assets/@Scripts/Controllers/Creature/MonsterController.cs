using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterController : EffectedCreature
{
    public BaseSkillBook Skills { get; protected set; }

    public Transform WayPoint { get; protected set; }

    public override void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                _animator.Play($"Idle");
                break;
            case Define.CreatureState.Moving:
                _animator.Play($"Moving");
                break;
            case Define.CreatureState.Casting:
                _animator.Play($"Casting");
                break;
            case Define.CreatureState.DoSkill:
                _animator.Play($"DoSkill");
                break;
            case Define.CreatureState.Dameged:
                _animator.Play($"Dameged");
                if (_coWait == null) Wait(0.75f); // OnDamege Animation 재생 wait // To Do Animation RunTime Parsing
                break;
            case Define.CreatureState.Dead:
                _animator.Play($"Death");
                break;
        }
    }

    protected override void UpdateDameged()
    {
        if (_coWait == null)
            CreatureState = Define.CreatureState.Moving;
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false; // 두 번 초기화하지 않도록 끊어주는 부분

        ObjectType = Define.ObjectType.Monster;
        CreatureState = Define.CreatureState.Moving;

        Skills = gameObject.GetOrAddComponent<BaseSkillBook>();

        //WayPoint = Managers.Game.WayPoints[Random.Range(0, Managers.Game.WayPoints.Count)];

        return true;
    }

    protected override void FixedUpdateMoving() // 물리와 연관돼 있으면
    {
        if (CreatureState != Define.CreatureState.Moving)
            return;

        /*if (WayPoint == null)
            return;*/
        PlayerController pc = Managers.Object.Player;
        if (pc.IsValid() == false)
            return;

        Vector3 dir = pc.transform.position - transform.position;

        MoveMonsterPosition(dir.normalized, _speed);

        // On Sprite Flip
        _spriteRenderer.flipX = dir.x < 0;
    }

    #region Move 함수들
    public virtual void MoveMonsterPosition(Vector3 dirNor, float speed)
    {
        Vector3 dir = dirNor * speed * Time.deltaTime;
        Vector3 newPos = transform.position + dir;

        GetComponent<Rigidbody2D>().MovePosition(newPos);
    }
    
    public float moveDistance { get; protected set; } = 0.0f;
    Coroutine _coMoveLength;
    public virtual void MoveMonsterPosition(Vector3 dirNor, float speed, float distance, Action onCompleteMove = null)
    {
        if (this.IsValid() == false)
            return;

        if (_coMoveLength != null)
            StopCoroutine(_coMoveLength);

        _coMoveLength = StartCoroutine(CoMoveLength(dirNor, speed, distance, onCompleteMove));
    }
    protected IEnumerator CoMoveLength(Vector3 dirNor, float speed, float distance, Action onCompleteMove = null)
    {
        while (distance > moveDistance)
        {
            if (this.IsValid() == false)
                yield break;

            Vector3 newPos = transform.position + dirNor * speed * Time.deltaTime;

            GetComponent<Rigidbody2D>().MovePosition(newPos);

            moveDistance += speed * Time.deltaTime;

            yield return null;
        }

        moveDistance = 0.0f;
        onCompleteMove.Invoke();

        StopCoroutine(_coMoveLength);
        _coMoveLength = null;
    }

    public virtual void MoveMonsterPosition(Transform creature, float speed)
    {
        Vector3 dir = transform.position - creature.position;
        Vector3 newPos = transform.position + dir.normalized * speed * Time.deltaTime;

        GetComponent<Rigidbody2D>().MovePosition(newPos);
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController target = collision.gameObject.GetComponent<PlayerController>();
        if (target.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;

        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);

        _coDotDamage = StartCoroutine(CoStartDotDamage(target));
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerController target = collision.gameObject.GetComponent<PlayerController>();
        if (target.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;

        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);
        _coDotDamage = null;
    }

    Coroutine _coDotDamage;
    public IEnumerator CoStartDotDamage(PlayerController target)
    {
        while (true)
        {
            // target.OnDamaged(this, 2); // 추후 논의 필요 > 몬스터가 공격 모션일 때만 Damage를 입힌다

            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void OnDead()
    {
        base.OnDead();

        Clear();

        Managers.Game.KillCount++;

        //죽을 떄 보석 스폰
        JamController jc = Managers.Object.Spawn<JamController>(transform.position);

        Managers.Object.Despawn(this);
    }

    protected override void Clear() // To Do : 초기화 내용 필요
    {
        StopAllCoroutines();

        #region 특정 Coroutine만 멈춰야할 경우 주석 처리한 방법으로 바꿀 것
        /*if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);
        _coDotDamage = null;*/
        #endregion
    }
}
