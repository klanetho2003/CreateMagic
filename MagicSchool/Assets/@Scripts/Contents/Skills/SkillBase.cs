using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using static Define;
using static AnimationEventManager;
using Unity.VisualScripting;

public abstract class SkillBase : MonoBehaviour // ��ų�� ���� > ActiveSkill �ߵ� >>> ��ų ����
{
    public CreatureController Owner { get; set; }
    public float RemainCoolTime { get; set; }

    public Data.SkillData SkillData { get; protected set; }
    public SkillBase CurrentSkill { get; protected set; }

    public virtual void SetInfo(CreatureController owner, int skillTemplateID)
    {
        Owner = owner;

        if (owner.ObjectType == EObjectType.Student)
            SkillData = Managers.Data.PlayerSkillDic[skillTemplateID];
        else
            SkillData = Managers.Data.MonsterSkillDic[skillTemplateID];

        // Handle AnimEvent
        if (Owner.Anim != null)
            BindEvent(Owner, OnAttackTargetHandler);
    }

    protected virtual void OnDisable() // ���� ����
    {
        Clear();
    }

    protected virtual void OnAttackTargetHandler()
    {
        if (Owner.CreatureState != CreatureState.DoSkill)
            return;

        if (CurrentSkill != this) // OnAttackEvent�� �����ϰ� �ִ� skill���� �ϰ� ���Ǵ� ���� �ذ� Temp
            return;

        AnimatorStateInfo currentAnim = Owner.Anim.GetCurrentAnimatorStateInfo(0);
        if (currentAnim.IsName(SkillData.AnimName) || currentAnim.IsName($"{SkillData.AnimName}_LookDown_{Owner.LookDown}"))
            OnAttackEvent();

        CurrentSkill = null;
    }

    protected virtual void FindTargets()
    {
        // To Do
    }

    protected abstract void OnAttackEvent();

    #region Init Method
    void Awake()
    {
        Init();
    }

    bool _init = false;

    public virtual bool Init() // ���� ������ ���� true�� ��ȯ, �� ���̶� ������ ������ ���� ��� false�� ��ȯ
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }
    #endregion
    
    public virtual void ActivateSkillOrDelay()
    {
        float delaySeconds = SkillData.ActivateSkillDelay;
        CurrentSkill = this;

        if (delaySeconds == 0)
            ActivateSkill();
        else if (delaySeconds > 0)
            OnSkillDelay(delaySeconds);
    }

    #region Activate Skill Or Delay

    public virtual void ActivateSkill()
    {
        Owner.CreatureState = CreatureState.DoSkill;
        

        if (Owner.ObjectType == EObjectType.Monster && SkillData.AnimName != null)
        {
            Owner.Anim.Play(SkillData.AnimName, -1, 0f);
            Owner.Skills.ActivateSkills.Remove(this);

            StartCoroutine(CoCountdownCooldown());
        }
        else // Player
        {
            string animName = $"{SkillData.AnimName}_LookDown_{Owner.LookDown}";
            Owner.Anim.Play(animName, -1, 0f);
        }
    }

    // Skill ��� �� ��������
    protected Coroutine _coOnSkillDelay;
    public virtual void OnSkillDelay(float delaySeconds)
    {
        if (_coOnSkillDelay != null)
            return;

        Owner.CreatureState = CreatureState.FrontDelay;
        _coOnSkillDelay = StartCoroutine(CoOnSkillDelay(delaySeconds));
    }
    IEnumerator CoOnSkillDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        if (Owner.CreatureState != CreatureState.FrontDelay)
        {
            StopCoroutine(_coOnSkillDelay);
            _coOnSkillDelay = null;
            yield return null;
        }

        ActivateSkill();
        _coOnSkillDelay = null;
    }

    #endregion

    public virtual void CancelSkill()
    {

    }

    protected virtual IEnumerator CoCountdownCooldown()
    {
        if (Owner.ObjectType != Define.EObjectType.Monster)
            yield break;

        Data.MonsterSkillData monsterData = (Data.MonsterSkillData)SkillData;

        RemainCoolTime = monsterData.CoolTime;
        yield return new WaitForSeconds(monsterData.CoolTime);
        RemainCoolTime = 0;

        // Ready Skill Add
        if (Owner.Skills != null)
            Owner.Skills.ActivateSkills.Add(this);
    }

    protected virtual ProjectileController GenerateProjectile(CreatureController onwer, Vector3 spawnPos, Action<BaseController> onHit = null)
    {
        ProjectileController projectile = Managers.Object.Spawn<ProjectileController>(spawnPos, SkillData.ProjectileId);

        // �浹�ϱ� ���� ģ���� settting
        LayerMask excludeMask = 0;
        excludeMask.AddLayer(ELayer.Default);
        excludeMask.AddLayer(ELayer.Projectile);
        excludeMask.AddLayer(ELayer.Env);
        excludeMask.AddLayer(ELayer.Obstacle);

        switch (Owner.ObjectType)
        {
            case EObjectType.Student:
                excludeMask.AddLayer(ELayer.Student);
                break;
            case EObjectType.Monster:
                excludeMask.AddLayer(ELayer.Monster);
                break;
        }

        projectile.SetSpawnInfo(Owner, this, spawnPos, excludeMask, onHit);

        return projectile;
    }

    public virtual void GenerateAoE(Vector3 spawnPos)
    {
        AoEBase aoe = null;
        int id = SkillData.AoEId;
        string className = Managers.Data.AoEDic[id].ClassName;

        Type componentType = Type.GetType(className);

        if (componentType == null)
        {
            Debug.LogError("AoE Type not found: " + className);
            return;
        }

        string spawnLabel = (Managers.Data.AoEDic[id].PrefabLabel != null) ? Managers.Data.AoEDic[id].PrefabLabel : "AoE";
        GameObject go = Managers.Object.SpawnGameObject(spawnPos, spawnLabel);
        go.name = Managers.Data.AoEDic[id].ClassName;
        aoe = go.AddComponent(componentType) as AoEBase;
        aoe.SetInfo(SkillData.AoEId, Owner, this);
    }

    #region Skill Delay

    //�ĵ�
    public void CompleteSkillDelay(float waitSeconds)
    {
        if (waitSeconds == 0)
        {
            Owner.CreatureState = Define.CreatureState.Idle;
            return;
        }

        if (_coOnSkillDelay != null)
            StopCoroutine(_coOnSkillDelay);

        _coOnSkillDelay = StartCoroutine(CoCompleteSkillDelay(waitSeconds));
    }

    IEnumerator CoCompleteSkillDelay(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        Owner.CreatureState = Define.CreatureState.Idle;
        _coOnSkillDelay = null;
    }
    #endregion

    protected virtual void Clear()
    {
        if (Managers.Game == null)
            return;

        if (Owner.IsValid() == true && Owner.Anim != null)
            UnbindEvent(Owner, OnAttackTargetHandler);
    }
}
