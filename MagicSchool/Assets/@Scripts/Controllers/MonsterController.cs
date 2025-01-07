using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    public BaseSkillBook Skills { get; protected set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false; // 두 번 초기화하지 않도록 끊어주는 부분

        ObjectType = Define.ObjectType.Monster;
        CreatureState = Define.CreatureState.Moving;

        Skills = gameObject.GetOrAddComponent<BaseSkillBook>();

        return true;
    }

    void FixedUpdate() // 물리와 연관돼 있으면
    {
        if (CreatureState != Define.CreatureState.Moving)
            return;

        PlayerController pc = Managers.Object.Player;
        if (pc == null)
            return;

        Vector3 dir = pc.transform.position - transform.position;
        Vector3 newPos = transform.position + dir.normalized * Time.deltaTime * _speed;

        GetComponent<Rigidbody2D>().MovePosition(newPos);
        _spriteRenderer.flipX = dir.x < 0;
    }

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

        //죽을 떄 보석 스폰
        JamController jc = Managers.Object.Spawn<JamController>(transform.position);

        Managers.Object.Despawn(this);
    }
}
