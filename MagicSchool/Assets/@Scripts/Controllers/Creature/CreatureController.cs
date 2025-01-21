using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : BaseController
{
    #region State Pattern

    Define.CreatureState _creatureState = Define.CreatureState.Idle;
    public virtual Define.CreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState == value)
                return;
            
            _creatureState = value;
            OnChangeState();
        }
    }

    protected virtual void OnChangeState()
    {
        if (this.IsValid() == false)
            return;
        UpdateAnimation();
    }

    protected Animator _animator;
    public virtual void UpdateAnimation() { }

    public override void UpdateController()
    {
        base.UpdateController();

        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                UpdateIdle();
                break;
            case Define.CreatureState.Moving:
                UpdateMoving();
                break;
            case Define.CreatureState.Casting:
                UpdateCasting();
                break;
            case Define.CreatureState.DoSkill:
                UpdateDoSkill();
                break;
            case Define.CreatureState.Dameged:
                UpdateDameged();
                break;
            case Define.CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateCasting() { }
    protected virtual void UpdateDoSkill() { }
    protected virtual void UpdateDameged() { }
    protected virtual void UpdateDead() { }

    #endregion

    #region Wait Coroutine
    protected Coroutine _coWait;

    protected virtual void Wait(float waitSeconds)
    {
        if (_coWait != null)
            StopCoroutine(_coWait);

        _coWait = StartCoroutine(CoWait(waitSeconds));
    }

    protected virtual IEnumerator CoWait(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        _coWait = null;
    }
    #endregion

    public override void FixedUpdateController()
    {
        FixedUpdateMoving();
    }

    protected virtual void FixedUpdateMoving() { }

    protected virtual void Moving() { }

    protected float _speed = 1.0f;

    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;

    protected SpriteRenderer _spriteRenderer;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        return true;
    }

    public virtual void OnDamaged(BaseController attacker, int damage)
    {
        if (this.IsValid() == false)
            return;
        if (Hp <= 0)
            return;

        CreatureState = Define.CreatureState.Dameged;
        Hp -= damage;

        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    protected virtual void OnDead()
    {

    }

    protected virtual void Clear()
    {

    }
}
