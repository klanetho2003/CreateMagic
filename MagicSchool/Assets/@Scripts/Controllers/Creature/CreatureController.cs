using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class CreatureController : BaseController
{
    #region State Methods

    public override void UpdateController()
    {
        base.UpdateController();

        switch (CreatureState)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Casting:
                UpdateCasting();
                break;
            case CreatureState.FrontDelay:
                UpdateDoSkill();
                break;
            case CreatureState.DoSkill:
                UpdateDoSkill();
                break;
            case CreatureState.BackDelay:
                UpdateDoSkill();
                break;
            case CreatureState.Dameged:
                UpdateDameged();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateCasting() { }
    protected virtual void UpdateFrontDelay() { }
    protected virtual void UpdateDoSkill() { }
    protected virtual void UpdateBackDelay() { }
    protected virtual void UpdateDameged() { }
    protected virtual void UpdateDead() { }

    #endregion

    #region Move
    
    public override void FixedUpdateController()
    {
        FixedUpdateMoving();
    }

    protected virtual void FixedUpdateMoving() { }

    protected virtual void Moving() { }

    #endregion

    public virtual BaseSkillBook Skills { get; protected set; }
    public virtual BaseController Target { get; protected set; } // player override
    public virtual Vector3 GenerateSkillPosition { get { return CenterPosition; } } // player override

    public Data.CreatureData CreatureData { get; private set; }
    public ECreatureType CreatureType { get; protected set; } = ECreatureType.None;

    public EffectComponent Effects { get; set; }

    #region Stats
    public float Hp { get; set; }
    public CreatureStat MaxHp;
    public CreatureStat Atk;
    public CreatureStat CriRate;
    public CreatureStat CriDamage;
    public CreatureStat ReduceDamageRate;
    public CreatureStat LifeStealRate;
    public CreatureStat ThornsDamageRate; // 쏜즈
    public CreatureStat MoveSpeed;
    public CreatureStat AttackSpeedRate;
    #endregion

    protected float AttackDistance
    {
        get
        {
            /*float env = 2.2f;
            if (Target != null && Target.ObjectType == EObjectType.Env)
                return Mathf.Max(env, Collider.radius + Target.Collider.radius + 0.1f);*/

            float baseValue = CreatureData.AtkRange;
            return baseValue;
        }
    }

    CreatureState _creatureState = CreatureState.Idle;
    public virtual CreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState == value)
                return;
            if (_creatureState == CreatureState.Dead)
                return;

            _creatureState = value;
            UpdateAnimation();
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Collider = gameObject.GetComponent<CircleCollider2D>();
        RigidBody = GetComponent<Rigidbody2D>();

        ObjectType = EObjectType.Creature;
        CreatureState = CreatureState.Idle;

        return true;
    }

    public virtual void SetInfo(int templateID)
    {
        DataTemplateID = templateID;

        if (CreatureType == ECreatureType.Student)
            CreatureData = Managers.Data.StudentDic[templateID];
        else
            CreatureData = Managers.Data.MonsterDic[templateID];

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.DescriptionTextID}"; // To Do : string data sheet

        // Collider
        Collider.offset = new Vector2(CreatureData.ColliderOffsetX, CreatureData.ColliderOffsetY);
        Collider.radius = CreatureData.ColliderRadius;

        // RigidBody
        RigidBody.mass = 0;

        // Material
        SpriteRenderer.material = Managers.Resource.Load<Material>(CreatureData.MaterialID);

        // Animatior
        SetAnimation(CreatureData.AnimatorDataID, CreatureData.SortingLayerName, SortingLayers.CREATURE);

        // Skills
        //CreatureData.SkillList; // 각 Controller SetInfo에서 초기화 하는 중

        // Stat
        Hp = CreatureData.MaxHp;
        MaxHp = new CreatureStat(CreatureData.MaxHp);
        Atk = new CreatureStat(CreatureData.Atk);
        CriRate = new CreatureStat(CreatureData.CriRate);
        CriDamage = new CreatureStat(CreatureData.Cridamage);
        ReduceDamageRate = new CreatureStat(0);
        LifeStealRate = new CreatureStat(0);
        ThornsDamageRate = new CreatureStat(0);
        MoveSpeed = new CreatureStat(CreatureData.MoveSpeed);
        AttackSpeedRate = new CreatureStat(1);


        // State
        CreatureState = CreatureState.Idle;

        // Effect
        Effects = gameObject.GetOrAddComponent<EffectComponent>();
        Effects.SetInfo(this);

        // Map Move
        //StartCoroutine(CoLerpToCellPos()); // MonsterController 내부 Update에서 하는 중
    }

    #region Wait
    protected Coroutine _coWait;

    public void StartWait(float seconds)
    {
        CancelWait();
        _coWait = StartCoroutine(CoWait(seconds));
    }

    IEnumerator CoWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coWait = null;
    }

    protected void CancelWait()
    {
        if (_coWait != null)
            StopCoroutine(_coWait);
        _coWait = null;
    }
    #endregion

    #region ETC

    protected bool IsValid(BaseController bc)
    {
        return bc.IsValid();
    }

    public virtual void SetRigidBodyVelocity(Vector2 velocity)
    {
        if (RigidBody == null)
            return;

        if (velocity.x < 0 && CreatureState != CreatureState.Casting)
            LookLeft = true;
        else if (velocity.x > 0 && CreatureState != CreatureState.Casting)
            LookLeft = false;

        if (velocity.y < 0)
            LookDown = true;
        else if (velocity.y > 0)
            LookDown = false;

        RigidBody.velocity = velocity;
    }

    #endregion

    #region Battle

    public void HandleDotDamage(EffectBase effect)
    {
        if (effect == null)
            return;
        /*if (effect.Owner.IsValid() == false) // DeBuff를 건 객체가 지속시간 이전에 Despanw되면 Debuff도 사라지게 할 것인가?
            return;*/

        // TEMP
        float damage = (Hp * effect.EffectData.PercentAdd) + effect.EffectData.Amount;
        if (effect.EffectData.ClassName.Contains("Heal"))
            damage *= -1f;

        float finalDamage = Mathf.Round(damage);
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp.Value);
        Managers.Object.ShowDamageFont(CenterPosition, finalDamage, transform, false);

        CreatureState = CreatureState.Dameged;

        // TODO : OnDamaged 통합
        if (Hp <= 0)
        {
            CreatureState = CreatureState.Dead;
            //OnDead(effect.Owner, effect.Skill);
            return;
        }
    }

    public override void OnDamaged(BaseController attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);

        if (attacker.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;

        CreatureController creature = attacker as CreatureController;
        if (creature == null)
            return;

        float finalDamage = creature.Atk.Value;
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp.Value);
        Managers.Object.ShowDamageFont(CenterPosition, finalDamage, transform);

        CreatureState = CreatureState.Dameged;

        if (Hp <= 0)
        {
            CreatureState = CreatureState.Dead;
            // OnDead()

            return;
        }

        // Skill에 따른 Effect 적용
        if (skill.SkillData.EffectIds != null)
            Effects.GenerateEffects(skill.SkillData.EffectIds.ToArray(), EEffectSpawnType.Skill, skill);

        // AOE
        if (skill != null && skill.SkillData.AoEId != 0)
            skill.GenerateAoE(transform.position);
    }

    #endregion

    #region Map

    public EFindPathResult FindPathAndMoveToCellPos(Vector3 destWorldPos, int maxDepth, bool forceMoveCloser = false)
    {
        Vector3Int destCellPos = Managers.Map.World2Cell(destWorldPos);
        return FindPathAndMoveToCellPos(destCellPos, maxDepth, forceMoveCloser);
    }

    public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
    {
        if (LerpCellPosCompleted == false)
            return EFindPathResult.Fail_LerpCell;

        // A*
        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destCellPos, maxDepth);
        if (path.Count < 2)
            return EFindPathResult.Fail_NoPath;

        if (forceMoveCloser)
        {
            Vector3Int diff1 = CellPos - destCellPos;
            Vector3Int diff2 = path[1] - destCellPos;
            if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
                return EFindPathResult.Fail_NoPath;
        }

        Vector3Int dirCellPos = path[1] - CellPos;
        //
        Vector3Int nextPos = CellPos + dirCellPos;

        if (Managers.Map.MoveTo(this, nextPos) == false)
            return EFindPathResult.Fail_MoveTo;

        return EFindPathResult.Success;
    }

    public bool MoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
    {
        if (LerpCellPosCompleted == false)
            return false;

        return Managers.Map.MoveTo(this, destCellPos);
    }

    // if use in Coroutine
    /*protected IEnumerator CoLerpToCellPos()
    {
        while (true)
        {
            Hero hero = this as Hero;
            if (hero != null)
            {
                float div = 5;
                Vector3 campPos = Managers.Object.Camp.Destination.transform.position;
                Vector3Int campCellPos = Managers.Map.World2Cell(campPos);
                float ratio = Math.Max(1, (CellPos - campCellPos).magnitude / div);

                LerpToCellPos(CreatureData.MoveSpeed * ratio);
            }
            else
                LerpToCellPos(CreatureData.MoveSpeed);

            yield return null;
        }
    }*/

    #endregion
}
