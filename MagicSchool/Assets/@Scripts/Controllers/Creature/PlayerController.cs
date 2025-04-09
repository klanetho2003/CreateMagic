using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class PlayerController : CreatureController
{
    public PlayerSkillBook PlayerSkills { get { return (PlayerSkillBook)Skills; } private set { base.Skills = value; } }
    public Data.StudentData PlayerData { get { return (Data.StudentData)CreatureData; } }
    public Data.StudentStatData PlayerStatData { get { return (Data.StudentStatData)CreatureStatData; } }

    #region Only Player Data
    public int Mp { get; set; }
    public CreatureStat MaxMp;
    #endregion

    #region Child Init

    Transform _indicator;
    Transform _fireSocket;
    Transform _shadow;
    SpriteRenderer _stemp;
    CinemachineVirtualCamera _cam;

    public Transform Shadow { get { return _shadow; } }
    public Transform Indicator { get { return _indicator; } }
    public Vector3 FireSocket { get { return _fireSocket.position; } }
    public Vector3 ShootDir { get { return (_fireSocket.position - _indicator.position).normalized; } }
    public CinemachineVirtualCamera Cam { get { return _cam; } }

    #endregion

    public override Vector3 GenerateSkillPosition { get => FireSocket; }

    Vector2 _moveDir = Vector2.zero;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            if (CreatureState == CreatureState.DoSkill) return;

            _moveDir = value.normalized;

            UpdateAnimation();
        }
    }

    public override CreatureState CreatureState
    {
        get { return base.CreatureState; }
        set
        {
            if (CreatureState == value)
                return;
            CreatureState lastState = CreatureState;

            base.CreatureState = value;

            switch (lastState)
            {
                case CreatureState.Casting:
                    InitShadow();
                    StopCoroutine(_coOnPlayCastingAnimation);
                    _coOnPlayCastingAnimation = null;
                    break;
                default:
                    break;
            }

            switch (value)
            {
                case CreatureState.Casting:
                    OnPlayCastingAnimation(5f, 0.0005f); // To Do : Data 시트 length는 홀수 여야한다(Cos주기 이슈)
                    break;
                case CreatureState.Dameged:
                    PlayerSkills.ClearCastingValue();
                    break;
                default:
                    break;

            }
        }
    }

    #region Player Animation

    protected override void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case CreatureState.Idle:
                {
                    if (LookDown)
                        Anim.Play("IdleFront");
                    else
                        Anim.Play("IdleBack");
                }
                break;
            case CreatureState.Moving:
                {
                    if (LookDown)
                        Anim.Play("MovingFront");
                    else
                        Anim.Play("MovingBack");
                }
                break;
            case CreatureState.Casting:
                {
                    if (LookDown)
                        Anim.Play("Casting_LookDown_True");
                    else
                        Anim.Play("Casting_LookDown_False");
                }
                break;
            case CreatureState.FrontDelay:
                break;
            case CreatureState.DoSkill:
                _stemp.flipX = !LookLeft;
                break;
            case CreatureState.BackDelay:
                break;
            case CreatureState.Dameged:
                {
                    SetDamagedMaterial();

                    if (LookDown)
                        Anim.Play("DamagedFront");
                    else
                        Anim.Play("DamagedBack");
                }
                break;
            case CreatureState.Dead:
                {
                    if (LookDown)
                        Anim.Play("DeathFront");
                    else
                        Anim.Play("DeathBack");
                }
                break;
        }
    }

    Coroutine _coOnPlayCastingAnimation;
    void OnPlayCastingAnimation(float speed, float length)
    {
        if (_coOnPlayCastingAnimation != null)
            StopCoroutine(_coOnPlayCastingAnimation);

        _coOnPlayCastingAnimation = StartCoroutine(CoOnPlayCastingAnimation(speed, length));
    }

    IEnumerator CoOnPlayCastingAnimation(float speed, float length)
    {
        float fixedTime = 0;

        _shadow.localPosition = new Vector2(_shadow.localPosition.x, _shadow.localPosition.y - 0.1f);

        while (true)
        {
            fixedTime += Time.deltaTime;

            Vector2 playerPosition = transform.localPosition;
            Vector2 shadowScale = _shadow.localScale;
            Vector2 shadowPosition = _shadow.localPosition;

            float weight = Mathf.Cos(fixedTime * speed) * length;

            Vector2 newPlayerPosition = new Vector2(playerPosition.x, playerPosition.y + weight);
            Vector2 newShadowScale = new Vector2(shadowScale.x - weight, shadowScale.y - weight);
            Vector2 newShadowPosition = new Vector2(shadowPosition.x, shadowPosition.y - weight * 1.5f);

            transform.localPosition = newPlayerPosition;
            _shadow.localScale = newShadowScale;
            _shadow.localPosition = newShadowPosition;

            yield return null;
        }
    }

    protected virtual void SetDamagedMaterial()
    {
        if (CreatureState == CreatureState.Dameged)
        {
            SpriteRenderer.material.EnableKeyword("HITEFFECT_ON");
        }
        else
        {
            SpriteRenderer.material.DisableKeyword("HITEFFECT_ON");
        }
    }

    #endregion

    #region Event Handling
    void HandleOnMoveDirChange(Vector2 dir)
    {
        MoveDir = dir;
    }

    void HandleOnKeyDown(KeyDownEvent key)
    {
        if (CreatureState == CreatureState.DoSkill)
            return;

        PlayerSkills.Command = key;
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        ObjectType = EObjectType.Student;

        // Event
        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChange;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChange; // 객체 참조값과 함께 함수를 전달하기에 가능한 구독
        Managers.Input.OnKeyDownHandler -= HandleOnKeyDown;
        Managers.Input.OnKeyDownHandler += HandleOnKeyDown;

        Collider.isTrigger = true;
        // RigidBody.simulated = false;

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        #region Only Player Stat Set
        Mp = 0;
        MaxMp = new CreatureStat(PlayerData.MaxMp);
        #endregion

        StartMpUp(PlayerData.MpGaugeAmount);

        #region Child Init
        _stemp = Utils.FindChild<SpriteRenderer>(gameObject, "Stemp", true);
        _stemp.enabled = false;
        _indicator = Utils.FindChild<Transform>(gameObject, "Indicator", true);
        _fireSocket = Utils.FindChild<Transform>(gameObject, "FireSocket", true);
        _shadow = Utils.FindChild<Transform>(gameObject, "Shadow", true);
        _cam = Utils.FindChild<CinemachineVirtualCamera>(gameObject, "Virtual Camera", true);
        #endregion

        AnimationEventManager.BindEvent(this, () =>
        {
            switch (CreatureState)
            {
                case CreatureState.Idle:
                    break;
                case CreatureState.Moving:
                    break;
                case CreatureState.Casting:
                    break;
                case CreatureState.FrontDelay:
                    break;
                case CreatureState.DoSkill:
                    break;
                case CreatureState.BackDelay:
                    break;
                case CreatureState.Dameged:
                    CreatureState = CreatureState.Idle;
                    SetDamagedMaterial();
                    break;
                case CreatureState.Stun:
                    SetDamagedMaterial();
                    break;
                case CreatureState.Dead:
                    break;
                default:
                    break;
            }
        });

        // Skill
        PlayerSkills = gameObject.GetOrAddComponent<PlayerSkillBook>();
        Skills.SetInfo(this, PlayerData);
    }


    void InitShadow()
    {
        _shadow.localPosition = new Vector2(_shadow.localPosition.x, 0f);
    }

    private void OnDestroy()
    {
        if (Managers.Game == null)
            return;
        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChange;
        Managers.Input.OnKeyDownHandler -= HandleOnKeyDown;

        CancleMpUp();
    }

    float timeTemp;
    public override void UpdateController()
    {
        base.UpdateController();

        /*timeTemp += Time.deltaTime;
        Debug.Log($"Mp = {Mp}, Time = {timeTemp}");*/
        //CollectEnv();
    }

    #region State Pattern

    protected override void UpdateIdle()
    {
        if (_moveDir != Vector2.zero) { CreatureState = CreatureState.Moving; return; }
    }

    protected override void UpdateMoving()
    {
        MoveIndicator();

        if (_moveDir == Vector2.zero) { CreatureState = CreatureState.Idle; return; }
    }

    protected override void UpdateCasting()
    {
        MoveIndicator();
    }

    protected override void UpdateDoSkill()
    {
        if (_coWait != null)
            return;

        CreatureState = CreatureState.Idle;
    }

    protected override void UpdateDameged()
    {

    }

    #endregion

    #region Move

    protected override void FixedUpdateMoving()
    {
        if (CreatureState != CreatureState.Moving && CreatureState != CreatureState.Casting && CreatureState != CreatureState.FrontDelay)
        {
            SetRigidBodyVelocity(Vector3.zero); // To Do : 길찾기
            return;
        }

        // 전방에 한 칸이 갈 수 있는 영역인가? - 빠른 탈출
        Vector3Int frontCellPos = Managers.Map.World2Cell(transform.position + (Vector3)_moveDir.normalized);
        if (Managers.Map.CanGo(this, frontCellPos, ignoreObjects: true) == false)
        {
            SetRigidBodyVelocity(Vector3.zero); // To Do : 길찾기
            return;
        }

        //실제 좌표 연산
        Vector3 dest = _moveDir.normalized * MoveSpeed.Value;
        Vector3Int destCellPos = Managers.Map.World2Cell(transform.position + (dest * Time.fixedDeltaTime));

        SetRigidBodyVelocity(dest);
        Managers.Map.MoveTo(this, destCellPos);
        Managers.Map.StageTransition.CheckMapChanged(destCellPos);
    }

    void MoveIndicator()
    {
        Vector3 dir = _moveDir * MoveSpeed.Value * Time.deltaTime;

        if (_moveDir == Vector2.zero)
            return;

        _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
    }

    #endregion

    #region Battle

    public override void OnDamaged(BaseController attacker, SkillBase skill)
    {
        if (this.CreatureState == CreatureState.Dameged)
            return;

        base.OnDamaged(attacker, skill);
        PlayerSkills.ClearCastingValue();
    }

    protected override void OnDead(BaseController attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);
    }

    #endregion

    #region Mp

    public Action OnMpGaugeUpStart;
    public Action<float> OnMpGaugeFill;
    public Action OnChangeTotalMpGauge;
    Coroutine _coStartMpUp;
    public void StartMpUp(float oneGaugeAmount)
    {
        // 방어
        if (MaxMp.Value <= Mp)
        {
            CancleMpUp();
            return;
        }

        if (_coStartMpUp != null)
            StopCoroutine(_coStartMpUp);

        _coStartMpUp = StartCoroutine(CoStartMpUp(oneGaugeAmount));
    }

    public IEnumerator CoStartMpUp(float oneGaugeAmount)
    {
        // 재시작
        while (OnMpGaugeUpStart == null)
            yield return null;

        // Gauge Start
        float currentMpGaugeAmount = 0;
        OnMpGaugeUpStart.Invoke(); // 널러블로 바꿔볼까

        while (this.IsValid() && MaxMp.Value > Mp)
        {
            currentMpGaugeAmount += Time.deltaTime;

            // Gauge 갱신
            OnMpGaugeFill.Invoke(currentMpGaugeAmount);

            if (currentMpGaugeAmount > oneGaugeAmount)
            {
                // Mp Up
                Mp += 1;

                StartMpUp(PlayerData.MpGaugeAmount); // 재시작
            }

            yield return null;
        }

        // 모두 차면 정지
        CancleMpUp();
    }

    public void CancleMpUp()
    {
        if (_coStartMpUp != null)
            StopCoroutine(_coStartMpUp);
        _coStartMpUp = null;
    }

    public override bool CheckChangeMp(int amount)
    {
        int sumMp = Mp + amount;

        if (sumMp < 0)
            return false;

        Mp = (int)Mathf.Clamp(sumMp, 0, MaxMp.Value);
        OnChangeTotalMpGauge.Invoke();

        if (_coStartMpUp == null && MaxMp.Value > Mp)
            StartMpUp(PlayerData.MpGaugeAmount);

        return true;
    }

    #endregion

    #region 주석화 - Collect Item
    // float EnvCollectDist { get; set; } = 1.0f;
    /* Temp Collect Env
    void CollectEnv()
    {
        float sqrCollectDist = EnvCollectDist * EnvCollectDist;

        List<JamController> jams = Managers.Object.Jams.ToList();

        var findJams = GameObject.Find("@Grid").GetComponent<GridController>().GatherObjects(transform.position, EnvCollectDist + 0.5f);

        foreach (var go in findJams)
        {
            JamController jam = go.GetComponent<JamController>();

            Vector3 dir = jam.transform.position - transform.position;
            if (dir.sqrMagnitude <= sqrCollectDist)
            {
                Managers.Game.Jam += 1;
                Managers.Object.Despawn(jam);
            }
        }
    }*/
    #endregion
}
