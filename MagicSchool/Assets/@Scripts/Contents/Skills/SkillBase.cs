using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using static Define;
using static AnimationEventManager;

public abstract class SkillBase : MonoBehaviour // ��ų�� ���� > ActiveSkill �ߵ� >>> ��ų ����
{
    public CreatureController Owner { get; set; }

    public Data.SkillData SkillData { get; protected set; }

    public virtual void SetInfo(CreatureController owner, int skillTemplateID)
    {
        Owner = owner;
        SkillData = Managers.Data.SkillDic[skillTemplateID];

        // Handle AnimEvent
        if (Owner.Anim != null)
        {
            BindEvent(Owner, "OnAttackTarget", OnAttackTargetHandler);
            BindEvent(Owner, "OnAnimComplate", OnAnimComplateHandler);
        }
    }

    private void OnDisable() // ���� ����
    {
        if (Managers.Game == null)
            return;
        if (Owner.IsValid() == false)
            return;
        if (Owner.Anim == null)
            return;

        UnbindEvent(Owner, "OnAttackTarget", OnAttackTargetHandler);
        UnbindEvent(Owner, "OnAnimComplate", OnAnimComplateHandler);
    }

    protected virtual void OnAttackTargetHandler()
    {

    }
    protected virtual void OnAnimComplateHandler()
    {

    }

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

    #region Activate Skill Or Delay
    
    public void ActivateSkillOrDelay()
    {
        Owner.CreatureState = CreatureState.DoSkill; // ���� �ڽ��� ActivateSkill �ȿ� �ִ� Code

        float delaySeconds = SkillData.ActivateSkillDelay;

        if (delaySeconds == 0)
            ActivateSkill();
        else if (delaySeconds > 0)
            OnSkillDelay(delaySeconds);
    }

    public virtual void ActivateSkill() {  }

    protected Coroutine _coOnSkillDelay;
    private void OnSkillDelay(float delaySeconds)
    {
        if (_coOnSkillDelay != null)
            return;

        _coOnSkillDelay = StartCoroutine(CoOnSkillDelay(delaySeconds));
    }
    IEnumerator CoOnSkillDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        ActivateSkill();
        _coOnSkillDelay = null;
    }

    #endregion


    // To Do : Generate �Լ� �ϳ��� ����
    protected virtual void GenerateProjectile(CreatureController onwer, Vector3 spawnPos, Action<BaseController> onHit)
    {
        ProjectileController projectile = Managers.Object.Spawn<ProjectileController>(spawnPos, SkillData.ProjectileId);

        // �浹�ϱ� ���� ģ���� settting
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
    }














    protected virtual RangeSkillController GenerateRangeSkill(Data.SkillData skillData, CreatureController onwer, float lifeTime, Vector3 spawnPos, Vector3 size, Action<CreatureController> OnHit = null)
    {
        /*RangeSkillController rc = Managers.Object.Spawn<RangeSkillController>(spawnPos, skillData.templateID);
        rc.SetInfo(skillData, Owner, lifeTime, size, OnHit);*/

        return null;//rc;
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

    #region Destory
    Coroutine _coDestory;

    public void StartDestory<T>(T bc, float delaySeconds) where T : BaseController
    {
        if (delaySeconds < 0)
            return;

        StopDestory();

        _coDestory = StartCoroutine(CoDestroy(bc, delaySeconds));
    }

    public void StopDestory()
    {
        if (_coDestory != null)
        {
            StopCoroutine(_coDestory);
            _coDestory = null;
        }
    }

    IEnumerator CoDestroy<T>(T bc, float delaySeconds) where T : BaseController
    {
        yield return new WaitForSeconds(delaySeconds);

        if (bc.IsValid())
        {
            Managers.Object.Despawn(bc);
        }
        /*else if (this.IsValid())
        {
            Managers.Object.Despawn(this);
        }*/
    }
    #endregion
}
