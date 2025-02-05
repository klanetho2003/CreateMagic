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

    #region Stats
    public float Hp { get; set; }
    public float MaxHp { get; set; }
    public float MaxHpBonusRate { get; set; }
    public float HealBonusRate { get; set; }
    public float HpRegen { get; set; }
    public float Atk { get; set; }
    public float AttackRate { get; set; }
    public float Def { get; set; }
    public float DefRate { get; set; }
    public float CriRate { get; set; }
    public float CriDamage { get; set; }
    public float DamageReduction { get; set; }
    public float MoveSpeedRate { get; set; }
    public float MoveSpeed { get; set; }
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
        Anim.runtimeAnimatorController = Managers.Resource.Load<RuntimeAnimatorController>(CreatureData.AnimatorDataID);
        SortingGroup sg = gameObject.GetOrAddComponent<SortingGroup>();
        sg.sortingLayerName = CreatureData.SortingLayerName;
        sg.sortingOrder = SortingLayers.CREATURE;

        // Skills
        //CreatureData.SkillList; //일단 skip

        // Stat
        MaxHp = CreatureData.MaxHp;
        Hp = CreatureData.MaxHp;
        Atk = CreatureData.Atk;
        MoveSpeed = CreatureData.MoveSpeed;


        // State
        CreatureState = CreatureState.Idle;

        // Map Move
        //StartCoroutine(CoLerpToCellPos());
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

    #endregion

    #region Battle

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

        float finalDamage = creature.Atk; // To Do * 피해량 * 크리티컬 등등
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);

        CreatureState = CreatureState.Dameged;

        if (Hp <= 0)
        {
            CreatureState = CreatureState.Dead;
            // OnDead()
        }
    }

    #endregion

    #region Move 구버전

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
