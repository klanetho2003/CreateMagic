using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class PlayerController : CreatureController
{
    float EnvCollectDist { get; set; } = 1.0f;

    public PlayerSkillBook _playerSkills;
    public override BaseSkillBook Skills
    {
        get { return _playerSkills as BaseSkillBook; }
        protected set { base.Skills = value; }
    }

    #region SerializeField in Prefab

    [SerializeField]
    Transform _skillBook;
    [SerializeField]
    Transform _indicator;
    [SerializeField]
    Transform _fireSocket;
    [SerializeField]
    Transform _shadow;
    [SerializeField]
    SpriteRenderer _stemp;

    public Transform SkillBook { get { return _skillBook; } }
    public Transform Shadow { get { return _shadow; } }
    public Transform Indicator { get { return _indicator; } }
    public Vector3 FireSocket { get { return _fireSocket.position; } }
    public Vector3 ShootDir { get { return (_fireSocket.position - _indicator.position).normalized; } }

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
                    {
                        StopCoroutine(_coOnPlayCastingAnimation);
                        _coOnPlayCastingAnimation = null;

                        InitShadow();
                    }// Stop CastingMoveMent, InitShadow
                    break;
                case CreatureState.DoSkill:
                    _isCompleteActive = false;
                    break;
                default:
                    break;
            }

            switch (value)
            {
                case CreatureState.Idle:
                    break;
                case CreatureState.Moving:
                    break;
                case CreatureState.Casting:
                    OnPlayCastingAnimation(5f, 0.0005f); // To Do : Data 시트 length는 홀수 여야한다(Cos주기 이슈)
                    break;
                case CreatureState.DoSkill:
                    //if (_coWait == null) Wait(0.45f); // 지팡이 휘두르기 재생 wait
                    break;
                case CreatureState.Dameged:
                    //if (_coWait == null) Wait(0.6f); // Damaged Animation 재생 wait
                    break;
                case CreatureState.Dead:
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
                        Anim.Play("CastingFront");
                    else
                        Anim.Play("CastingBack");
                }
                break;
            case CreatureState.DoSkill:
                {
                    if (LookDown)
                        Anim.Play("DoSkillFront");
                    else
                        Anim.Play("DoSkillBack");
                }
                break;
            case CreatureState.Dameged:
                {
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

        _playerSkills.Command = key;
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        CreatureType = ECreatureType.Student;

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        _stemp = SpriteRenderer; // ?

        // Event
        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChange;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChange; // 객체 참조값과 함께 함수를 전달하기에 가능한 구독
        Managers.Input.OnKeyDownHandler -= HandleOnKeyDown;
        Managers.Input.OnKeyDownHandler += HandleOnKeyDown;

        AnimationEventManager.BindEvent(this, "OnDamaged_Complate", () =>
        {
            if (CreatureState != CreatureState.Dameged)
                return;

            CreatureState = CreatureState.Idle;
            SetDamagedMaterial();
        });

        // Skill
        _playerSkills = gameObject.GetOrAddComponent<PlayerSkillBook>();
        Skills.SetInfo(this, CreatureData.SkillList);
    }


    void InitShadow()
    {
        _shadow.localPosition = new Vector2(_shadow.localPosition.x, -0.55f);
    }

    private void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChange;
    }
    
    public override void UpdateController()
    {
        MoveIndicator();

        base.UpdateController();

        CollectEnv();
    }

    #region State Pattern

    protected override void UpdateIdle()
    {
        if (_playerSkills.InputQueue.Count > 0)
            _playerSkills.InputQueue.Clear();

        if (_moveDir != Vector2.zero) { CreatureState = Define.CreatureState.Moving; return; }
    }

    protected override void UpdateMoving()
    {
        if (_moveDir == Vector2.zero) { CreatureState = Define.CreatureState.Idle; return; }
    }

    protected override void UpdateCasting()
    {
        
    }

    bool _isCompleteActive = false;
    protected override void UpdateDoSkill()
    {
        if (_isCompleteActive == true)
            return;

        /*if (_coWait == null)
            _isCompleteActive = PlayerSkills.ActiveSkill();*/
    }

    protected override void UpdateDameged()
    {
        /*if (_coWait == null)
        {
            CreatureState = CreatureState.Idle;
            SetDamagedMaterial();
        }*/
    }

    #endregion

    #region Move

    protected override void FixedUpdateMoving()
    {
        if (CreatureState != CreatureState.Moving && CreatureState != CreatureState.Casting)
        {
            SetRigidBodyVelocity(Vector3.zero); // To Do : 길찾기
            return;
        }

        SetRigidBodyVelocity(_moveDir.normalized * MoveSpeed);
    }

    void MoveIndicator()
    {
        Vector3 dir = _moveDir * MoveSpeed * Time.deltaTime;

        if (_moveDir == Vector2.zero)
            return;

        _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
    }

    #endregion

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
    }

    #region Battle

    public override void OnDamaged(BaseController attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);

        SetDamagedMaterial();
    }

    protected override void OnDead(BaseController attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);
    }

    #endregion
}
