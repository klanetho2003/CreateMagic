using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
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
                break;
            case Define.CreatureState.Dead:
                _animator.Play($"Death");
                break;
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false; // �� �� �ʱ�ȭ���� �ʵ��� �����ִ� �κ�

        ObjectType = Define.ObjectType.Monster;
        CreatureState = Define.CreatureState.Moving;

        Skills = gameObject.GetOrAddComponent<BaseSkillBook>();

        //WayPoint = Managers.Game.WayPoints[Random.Range(0, Managers.Game.WayPoints.Count)];

        return true;
    }

    protected override void FixedUpdateMoving() // ������ ������ ������
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

    public virtual void MoveMonsterPosition(Vector3 dirNor, float speed)
    {
        Vector3 dir = dirNor * speed * Time.deltaTime;
        Vector3 newPos = transform.position + dir;

        GetComponent<Rigidbody2D>().MovePosition(newPos);
    }

    Coroutine _coMoveLength;
    public float moveLength { get; protected set; }
    public virtual bool MoveMonsterPosition(Vector3 dirNor, float speed, float length)
    {
        Vector3 dir = dirNor * speed * Time.deltaTime;
        Vector3 newPos = transform.position + dir;

        GetComponent<Rigidbody2D>().MovePosition(newPos);

        moveLength += speed * Time.deltaTime;

        return (moveLength == length) ? true : false; // To Use >> if (MoveMonsterPosition(,,,)) return;
    }

    // To Do : Ư�� ��ġ���� �̵��ϴ� ���� ���������, Coroutine���� �ݺ��ϴ� ���� ���� ��
    protected IEnumerator CoMoveLength()
    {
        yield return null;
    }

    //� ��ġ�� �̵�
    /*public virtual void MoveMonsterPosition(Vector3 destPosition, float speed)
    {
        Vector3 dir = transform.position - destPosition;
        Vector3 newPos = transform.position + dir.normalized * speed * Time.deltaTime;

        GetComponent<Rigidbody2D>().MovePosition(newPos);
    }*/

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
            target.OnDamaged(this, 2);

            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void OnDead()
    {
        base.OnDead();

        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);
        _coDotDamage = null;

        Managers.Game.KillCount++;

        //���� �� ���� ����
        JamController jc = Managers.Object.Spawn<JamController>(transform.position);

        Managers.Object.Despawn(this);
    }
}
