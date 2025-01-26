using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class MonsterController : EffectedCreature
{
    public BaseSkillBook Skills { get; protected set; }

    public Transform WayPoint { get; protected set; }

    BaseController _target;
    public float AttaccDistence
    {
        get
        {
            // radius 값은 SetInfo하면서 부여하는 값이기에 Data Parsing을 구현하면변수로 존재할 것
            float targetRadius = (_target.IsValid() == true) ? _target.ColliderRadius : 0;
            return targetRadius + ColliderRadius + MONSTER_DEFAULT_MELEE_ATTACK_RANGE; // 2.0f은 추가 판정값 : To Do DataParsing
        }
    }

    #region Animation

    public override void FlipX(bool flag) // Monster는 태생이 오른쪽, 반대로 값 넣
    {
        flag = !flag;
        base.FlipX(flag);
    }

    // Wait와 Animation을 묶는 함수 BaseController에 넣자
    protected override void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case CreatureState.Idle:
                Anim.Play($"Idle");
                break;
            case CreatureState.Moving:
                Anim.Play($"Moving");
                break;
            case CreatureState.Casting:
                Anim.Play($"Casting");
                break;
            case CreatureState.DoSkill:
                Anim.Play($"DoSkill");
                Wait(1f); // Damege Animation + 후딜레이 재생 wait // To Do Animation RunTime Parsing
                break;
            case CreatureState.Dameged:
                Anim.Play($"Dameged");
                Wait(0.75f); // Damege Animation + 후딜레이 재생 wait // To Do Animation RunTime Parsing
                break;
            case CreatureState.Dead:
                Anim.Play($"Death");
                Wait(1.5f); // Death Animation + 후딜레이 재생 wait // To Do Animation RunTime Parsing
                break;
        }
    }

    #endregion

    #region AI - by StatePatern

    protected override void UpdateIdle()
    {
        if (_target.IsValid() == true)
            CreatureState = CreatureState.Moving;
    }

    protected override void UpdateMoving()
    {
        if (_target.IsValid() == false)
        {
            CreatureState = CreatureState.Idle;
            return;
        }

        // CheckAttackTarget() in FixedUpdateMoving - MonsterController
    }

    protected override void UpdateDoSkill()
    {
        if (_target.IsValid() == false)
        {
            CreatureState = CreatureState.Idle;
            return;
        }

        if (_coWait == null)
        {
            Vector3 dir = _target.transform.position - transform.position;
            CreatureState = CheckAttackTarget(dir.sqrMagnitude);
        }
            
    }

    protected override void UpdateDameged()
    {
        if (_coWait == null)
            CreatureState = CreatureState.Moving;
    }

    protected override void UpdateDead()
    {
        if (_coWait == null)
            OnDead();
    }

    CreatureState CheckAttackTarget(float sqrMagnitude)
    {
        float distTargetSqr = sqrMagnitude;
        float attackDistenceSqr = AttaccDistence * AttaccDistence;

        return (distTargetSqr <= attackDistenceSqr) ? CreatureState.DoSkill : CreatureState.Moving;
    }

    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false; // 두 번 초기화하지 않도록 끊어주는 부분

        CreatureType = ECreatureType.Monster;

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        CreatureState = CreatureState.Moving;

        _target = Managers.Object.Player;

        Skills = gameObject.GetOrAddComponent<BaseSkillBook>();

        AnimationEventManager.BindEvent(this, "OnAttackTarget", OnAttackTargetHandler);

        //WayPoint = Managers.Game.WayPoints[Random.Range(0, Managers.Game.WayPoints.Count)];
    }

    public virtual void OnAttackTargetHandler()
    {
        Debug.Log($"Attacker : {gameObject.name}, Target : {_target.name}, Damage : {10}");

        if (_target.IsValid() == false)
            return;

        _target.OnDamaged(this, (int)Atk);
    }

    protected override void FixedUpdateMoving() // 물리와 연관돼 있으면
    {
        if (CreatureState != CreatureState.Moving)
        {
            SetRigidBodyVelocity(Vector3.zero); // To Do : 길찾기
            return;
        }

        /*if (WayPoint == null)
            return;*/
        
        if (_target.IsValid() == false)
            return;

        Vector3 dir = _target.transform.position - transform.position;

        CreatureState = CheckAttackTarget(dir.sqrMagnitude);

        SetRigidBodyVelocity(dir.normalized * MoveSpeed);
    }

    #region Move Methods
    
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

    #region Battle

    public override void OnDamaged(BaseController attacker, int damage)
    {
        base.OnDamaged(attacker, damage);
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

    #endregion

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
