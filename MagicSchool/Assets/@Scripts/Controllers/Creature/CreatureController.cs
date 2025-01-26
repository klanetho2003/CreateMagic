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
            case CreatureState.DoSkill:
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
    protected virtual void UpdateDoSkill() { }
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

    public BaseController Target { get; protected set; }
    public BaseSkillBook Skills { get; protected set; }

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

        gameObject.name = $"{CreatureData.TemplateID}_{CreatureData.DescriptionTextID}"; // To Do : string data sheet

        // Collider
        Collider.offset = new Vector2(CreatureData.ColliderOffsetX, CreatureData.ColliderOffsetY);
        Collider.radius = CreatureData.ColliderRadius;

        // RigidBody
        RigidBody.mass = CreatureData.Mass;

        // Material
        SpriteRenderer.material = Managers.Resource.Load<Material>(CreatureData.MaterialID);

        // Animatior
        Anim.runtimeAnimatorController = Managers.Resource.Load<RuntimeAnimatorController>(CreatureData.AnimatorDataID);
        SortingGroup sg = gameObject.GetOrAddComponent<SortingGroup>();
        sg.sortingLayerName = CreatureData.SortingLayerName;
        sg.sortingOrder = SortingLayers.CREATURE;

        // Skills
        // CreatureData.SkillList; //일단 skip

        // Stat
        MaxHp = CreatureData.MaxHp;
        Hp = CreatureData.MaxHp;
        Atk = CreatureData.Atk;
        MoveSpeed = CreatureData.MoveSpeed;


        // State
        CreatureState = CreatureState.Idle;
    }

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
}
