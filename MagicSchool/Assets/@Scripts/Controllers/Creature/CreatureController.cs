using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
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
            case CreatureState.Spawning:
                UpdateSpawning();
                break;
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

    protected virtual void UpdateSpawning() { }
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
    public Data.CreatureStatData CreatureStatData { get; private set; }
    public Dictionary<ResistType, CreatureStat> CreatureResistDic = new Dictionary<ResistType, CreatureStat>();

    public EffectComponent Effects { get; set; }
    public Sprite LastStateSprite { get; private set; }

    #region Stats
    // Temp
    public Action<float> OnHpChange;
    float _hpTemp;
    public float Hp
    {
        get { return _hpTemp; }
        set { _hpTemp = Mathf.Clamp(value, 0, MaxHp.Value); OnHpChange?.Invoke(_hpTemp); }
    }
    //

    //public float Hp { get; set; }
    public CreatureStat MaxHp;
    public CreatureStat Atk;
    public CreatureStat MoveSpeed;
    public CreatureStat CriRate;
    public CreatureStat CriDamage;

    public CreatureStat FireResist;
    public CreatureStat IceResist;
    public CreatureStat ElectricResist;
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

    [SerializeField]
    CreatureState _creatureState = CreatureState.Idle;
    public virtual CreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState == value)
                return;

            LastStateSprite = SpriteRenderer.sprite;

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

        CreatureState = CreatureState.Idle;

        return true;
    }

    public virtual void SetInfo(int templateID)
    {
        DataTemplateID = templateID;

        if (ObjectType == EObjectType.Student)
        {
            CreatureData = Managers.Data.StudentDic[templateID];
            CreatureStatData = Managers.Data.StudentStatDic[templateID];
        }
        else
        {
            CreatureData = Managers.Data.MonsterDic[templateID];
            CreatureStatData = Managers.Data.MonsterStatDic[templateID];
        }

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

        #region Stat
        MaxHp = new CreatureStat(CreatureStatData.MaxHp);
        Hp = CreatureStatData.MaxHp;
        Atk = new CreatureStat(CreatureStatData.Atk);
        MoveSpeed = new CreatureStat(CreatureStatData.MoveSpeed);
        CriRate = new CreatureStat(CreatureStatData.CriRate);
        CriDamage = new CreatureStat(CreatureStatData.CriDamageMult);

        AddResist((int)ResistType.Melee, FireResist);
        AddResist((int)ResistType.Fire, FireResist);
        AddResist((int)ResistType.Ice, IceResist);
        AddResist((int)ResistType.Electric, ElectricResist);
        #endregion

        // State
        CreatureState = CreatureState.Idle;

        // Effect
        Effects = gameObject.GetOrAddComponent<EffectComponent>();
        Effects.SetInfo(this);

        // HpBar - Temp
        UI_World_HpBar hpBar_temp = gameObject.FindChild<UI_World_HpBar>(recursive: false);
        if (hpBar_temp == null)
            hpBar_temp = Managers.UI.MakeWorldSpaceUI<UI_World_HpBar>(this.transform);
        hpBar_temp.SetInfo(this);

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
        RigidBody.velocity = velocity;

        if (CreatureState != CreatureState.Moving && CreatureState != CreatureState.Casting)
            return;

        if (velocity.x < 0)
            LookLeft = true;
        else if (velocity.x > 0)
            LookLeft = false;

        if (velocity.y < 0)
            LookDown = true;
        else if (velocity.y > 0)
            LookDown = false;
    }

    #endregion

    #region Battle

    void AddResist(int type, CreatureStat resist)
    {
        resist = new CreatureStat(CreatureStatData.ResistData[type].value);
        CreatureResistDic.Add(CreatureStatData.ResistData[type].ResistType, resist);
    }

    public float GetResistance(ResistType type)
    {
        return CreatureResistDic.TryGetValue(type, out CreatureStat Stat) ? Stat.Value : 0f;
    }

    public void HandleDotDamage(SkillBase skill, EffectBase effect)
    {
        if (effect == null)
            return;
        if (this.IsValid() == false)
            return;
        /*if (effect.Owner.IsValid() == false) // DeBuff를 건(?) 객체가 지속시간 이전에 Despanw되면 Debuff도 사라지게 할 것인가?
            return;*/

        // TEMP
        /*float damage = (Hp * effect.EffectData.PercentAdd) + effect.EffectData.Amount;
        if (effect.EffectData.ClassName.Contains("Heal"))
            damage *= -1f;*/

        float rawDamage = skill.Owner.Atk.Value * effect.EffectData.PercentAdd;
        ResistType resisType = skill.SkillData.SkillType;
        float sumDamage = rawDamage * (1f - GetResistance(resisType));

        float finalDamage = Mathf.Round(sumDamage);
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp.Value);
        Managers.Object.ShowDamageFont(CenterPosition, finalDamage, transform, false);

        CreatureState = CreatureState.Dameged;

        // TODO : OnDamaged 통합
        if (Hp <= 0)
        {
            CreatureState = CreatureState.Dead;
            OnDead(effect.Owner, effect.Skill);
            return;
        }
    }

    public override void ApplyDamage(BaseController attacker, SkillBase skill)
    {
        base.ApplyDamage(attacker, skill);

        if (this.IsValid() == false)
            return;

        CreatureState = CreatureState.Dameged;

        CreatureController creature = attacker as CreatureController;
        if (creature == null)
            return;

        float rawDamage = creature.Atk.Value * skill.SkillData.DamageMultiplier;
        ResistType resisType = skill.SkillData.SkillType;
        float finalDamage = rawDamage * (1f - GetResistance(resisType));
        Hp = Hp - finalDamage; // Clamp in Property
        Managers.Object.ShowDamageFont(CenterPosition, finalDamage, transform);

        // OnDead
        if (Hp <= 0)
        {
            Anim.speed = 1; // Hard? Dead Animation 안에 Despawn Event가 있기에 예외로 처리
            CreatureState = CreatureState.Dead;
            OnDead(attacker, skill);
            return;
        }
    }

    public override void OnDamaged(BaseController attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);

        if (attacker.IsValid() == false)
            return;

        // AOE
        if (skill != null && skill.SkillData.AoEId != 0)
            skill.GenerateAoE(transform.position);

        // Skill에 따른 Effect 적용
        if (skill.SkillData.EffectIds != null)
            Effects.GenerateEffects(skill.SkillData.EffectIds.ToArray(), EEffectSpawnType.Skill, skill);

        // Sum Damage
        ApplyDamage(attacker, skill);
    }

    public virtual bool CheckChangeMp(int amount)
    {
        return ObjectType == EObjectType.Monster;
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
        List<Vector3Int> path = Managers.Map.FindPath(this, CellPos, destCellPos, maxDepth);
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
