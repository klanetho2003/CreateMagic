using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using static Define;
using static AnimationEventManager;
using Unity.VisualScripting;

public abstract class SkillBase : MonoBehaviour // 스킬을 스폰 > ActiveSkill 발동 >>> 스킬 시전
{
    public CreatureController Owner { get; set; }

    public Data.SkillData SkillData { get; protected set; }

    public virtual void SetInfo(CreatureController owner, int skillTemplateID)
    {
        Owner = owner;
        SkillData = Managers.Data.SkillDic[skillTemplateID];

        // Handle AnimEvent
        if (Owner.Anim != null)
            BindEvent(Owner, OnAttackTargetHandler);
    }

    private void OnDisable() // 게임 강종
    {
        if (Managers.Game == null)
            return;
        if (Owner.IsValid() == false)
            return;
        if (Owner.Anim == null)
            return;

        UnbindEvent(Owner, OnAttackTargetHandler);
    }

    protected virtual void OnAttackTargetHandler()
    {
        if (Owner.CreatureState != CreatureState.DoSkill)
            return;
    }

    #region Init Method
    void Awake()
    {
        Init();
    }

    bool _init = false;

    public virtual bool Init() // 최초 실행일 떄는 true를 반환, 한 번이라도 실행한 내역이 있을 경우 false를 반환
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }
    #endregion

    #region Activate Skill Or Delay
    
    public void ActivateSkillOrDelay()
    {
        float delaySeconds = SkillData.ActivateSkillDelay;

        if (delaySeconds == 0)
            ActivateSkill();
        else if (delaySeconds > 0)
            OnSkillDelay(delaySeconds);
    }
    
    public virtual void ActivateSkill()
    {
        Owner.CreatureState = CreatureState.DoSkill;

        if (Owner.CreatureType == ECreatureType.Monster && SkillData.AnimName != null)
            Owner.Anim.Play(SkillData.AnimName);
        else // Player
        {
            string animName = $"{SkillData.AnimName}_LookDown_{Owner.LookDown}";
            Owner.Anim.Play(animName);
        }
    }

    // Skill 사용 전 선딜레이
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

    protected virtual ProjectileController GenerateProjectile(CreatureController onwer, Vector3 spawnPos, Action<BaseController, Vector3> onHit, string prefabLab = null)
    {
        ProjectileController projectile;
        if (prefabLab == null)
            projectile = Managers.Object.Spawn<ProjectileController>(spawnPos, SkillData.ProjectileId);
        else
            projectile = Managers.Object.Spawn<ProjectileController>(spawnPos, SkillData.AfterSkillId, prefabLab);

        // 충돌하기 싫은 친구들 settting
        LayerMask excludeMask = 0;
        excludeMask.AddLayer(ELayer.Default);
        excludeMask.AddLayer(ELayer.Projectile);
        excludeMask.AddLayer(ELayer.Env);
        excludeMask.AddLayer(ELayer.Obstacle);

        switch (Owner.CreatureType)
        {
            case ECreatureType.Student:
                excludeMask.AddLayer(ELayer.Student);
                break;
            case ECreatureType.Monster:
                excludeMask.AddLayer(ELayer.Monster);
                break;
        }

        projectile.SetSpawnInfo(Owner, this, excludeMask, onHit);

        return projectile;
    }

    #region Skill Delay

    //후딜
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
}
