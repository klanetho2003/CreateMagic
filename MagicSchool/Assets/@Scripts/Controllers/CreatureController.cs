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
            _creatureState = value;
            UpdateAnimation();
        }
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
            case Define.CreatureState.Skill:
                UpdateSkill();
                break;
            case Define.CreatureState.Moving:
                UpdateMoving();
                break;
            case Define.CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateDead() { }

    #endregion

    protected float _speed = 1.0f;

    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;

    protected SpriteRenderer _spriteRenderer;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _spriteRenderer = GetComponent<SpriteRenderer>();

        return true;
    }

    public virtual void OnDamaged(BaseController attacker, int damage)
    {
        if (Hp <= 0)
            return;

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
}
