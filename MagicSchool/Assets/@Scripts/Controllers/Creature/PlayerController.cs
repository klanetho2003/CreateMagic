using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class PlayerController : CreatureController
{
    float EnvCollectDist { get; set; } = 1.0f;

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

    public PlayerSkillBook Skills { get; protected set; }

    bool _isFront = true;
    Vector2 _moveDir = Vector2.zero;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            if (CreatureState == CreatureState.DoSkill) return;

            if (_moveDir == value.normalized) // �ߺ��� �� �� ������ lastDir�� �ٲ�� ���� ����
                return;

            Vector2 lastDir = _moveDir;
            _moveDir = value.normalized;

            #region �ִϸ��̼� Update
            if (value.x == 0 && value.y == 0)
                return;

            bool IsFlip = (value.x == 0) ? lastDir.x > 0 : value.x > 0;
            _spriteRenderer.flipX = IsFlip;
            _stemp.flipX = IsFlip;

            if (value.y != 0) { _isFront = value.y < 0; }
            UpdateAnimation();
            #endregion
        }
    }

    public override CreatureState CreatureState
    {
        get { return base.CreatureState; }
        set
        {
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
                    OnPlayCastingAnimation(5f, 0.0005f); // To Do : Data ��Ʈ length�� Ȧ�� �����Ѵ�(Cos�ֱ� �̽�)
                    break;
                case CreatureState.DoSkill:
                    break;
                case CreatureState.Dameged:
                    break;
                case CreatureState.Dead:
                    break;
                default:
                    break;
            }
        }
    }

    #region Player Animation
    public override void UpdateAnimation()
    {
        string dir = (_isFront == true) ? "Front" : "Back";

        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                _animator.Play($"Idle{dir}");
                break;
            case Define.CreatureState.Moving:
                _animator.Play($"Moving{dir}");
                break;
            case Define.CreatureState.Casting:
                _animator.Play($"Casting{dir}");
                break;
            case Define.CreatureState.DoSkill:
                _animator.Play($"DoSkill{dir}");
                if (_coWait == null) Wait(0.45f); // ������ �ֵθ��� ��� wait
                break;
            case Define.CreatureState.Dameged:
                _animator.Play($"Dameged{dir}");
                break;
            case Define.CreatureState.Dead:
                _animator.Play($"Death{dir}");
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
    #endregion

    #region Event Hadling
    void HandleOnMoveDirChange(Vector2 dir)
    {
        MoveDir = dir;
    }

    void HandleOnKeyDown(Define.KeyDownEvent key)
    {
        if (CreatureState == Define.CreatureState.DoSkill)
            return;

        Skills.Inputkey = key;
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.Data.PlayerDic.TryGetValue(1, out Data.PlayerData playerData);
        _speed = playerData.speed;

        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChange; // ��ü �������� �Բ� �Լ��� �����ϱ⿡ ������ ����
        Managers.Input.OnKeyDownHandler += HandleOnKeyDown;

        Skills = gameObject.GetOrAddComponent<PlayerSkillBook>();
        _stemp = gameObject.GetOrAddComponent<SpriteRenderer>();

        ObjectType = Define.ObjectType.Player;
        CreatureState = Define.CreatureState.Idle;

        // To Do
        FireBallSkill fireBallSkill = Skills.AddSkill<FireBallSkill>(_indicator.position, SkillBook);

        return true;
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

        if (_coWait == null)
            _isCompleteActive = Skills.ActiveSkill();
    }

    #endregion

    #region Move
    
    protected override void FixedUpdateMoving()
    {
        if (CreatureState != Define.CreatureState.Moving && CreatureState != Define.CreatureState.Casting)
            return;

        MovePlayer();
    }

    protected void MovePlayer()
    {
        Vector3 dir = _moveDir * _speed * Time.deltaTime;
        transform.position += dir;

        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    void MoveIndicator()
    {
        Vector3 dir = _moveDir * _speed * Time.deltaTime;

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

    public override void OnDamaged(BaseController attacker, int damage)
    {
        base.OnDamaged(attacker, damage);
    }

    protected override void OnDead()
    {
        base.OnDead();
    }

    #endregion
}
