using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class PlayerController : CreatureController
{
    float EnvCollectDist { get; set; } = 1.0f;

    [SerializeField]
    Transform _indicator;
    [SerializeField]
    Transform _fireSocket;
    [SerializeField]
    Transform _shadow;
    [SerializeField]
    SpriteRenderer _stemp;

    public Transform Indicator { get { return _indicator; } }
    public Vector3 FireSocket { get { return _fireSocket.position; } }
    public Vector3 ShootDir { get { return (_fireSocket.position - _indicator.position).normalized; } }

    public PlayerSkillBook Skills { get; protected set; }

    bool _isFront = true;
    Vector2 _moveDir = Vector2.zero;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            if (CreatureState == CreatureState.DoSkill) return;

            if (_moveDir == value.normalized) // 중복일 때 두 번들어가서 lastDir이 바뀌는 것을 방지
                return;

            Vector2 lastDir = _moveDir;
            _moveDir = value.normalized;

            #region 애니메이션 Update
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

    protected override void OnChangeState()
    {
        base.OnChangeState();

        if (_isDoSkill == true)
            _isDoSkill = false;

        switch (CreatureState)
        {
            case CreatureState.Casting:
                {
                    OnPlayCastingAnimation(5f, 0.0005f); // To Do : Data 시트 length는 홀수 여야한다(Cos주기 이슈)
                }
                break;
            default:
                {
                    if (_coOnPlayCastingAnimation != null)
                    {
                        StopCoroutine(_coOnPlayCastingAnimation);
                        _coOnPlayCastingAnimation = null;

                        InitShadow();
                    }
                }
                break;
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
                if (_coWait == null) Wait(0.45f); // 지팡이 휘두르기 재생 wait
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

        CreatureState = Skills.BuildSKillKey($"{key}");
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.Data.PlayerDic.TryGetValue(1, out Data.PlayerData playerData);
        _speed = playerData.speed;

        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChange; // 객체 참조값과 함께 함수를 전달하기에 가능한 구독
        Managers.Input.OnKeyDownHandler += HandleOnKeyDown;

        Skills = gameObject.GetOrAddComponent<PlayerSkillBook>();
        _stemp = gameObject.GetOrAddComponent<SpriteRenderer>();

        ObjectType = Define.ObjectType.Player;
        CreatureState = Define.CreatureState.Idle;

        // To Do
        FireBallSkill fireBallSkill = Skills.AddSkill<FireBallSkill>(_indicator.position, transform); //받아서 추가 수정 가능
        Skills.AddSkill<EgoSword>(_indicator.position, transform);

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

    bool _isDoSkill = false;
    protected override void UpdateDoSkill()
    {
        if (_isDoSkill == true)
            return;

        if (_coWait == null)
            _isDoSkill = Skills.ActiveSkill();
    }

    #region Wait Coroutine
    Coroutine _coWait;

    void Wait(float waitSeconds)
    {
        if (_coWait != null)
            StopCoroutine(_coWait);

        _coWait = StartCoroutine(CoWait(waitSeconds));
    }

    IEnumerator CoWait(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        _coWait = null;
    }
    #endregion

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MonsterController target = collision.gameObject.GetComponent<MonsterController>();
        if (target.IsValid() == false)
            return;

        // To Do : 닿기만 해도 피격 판정이 있으면 여기에 OnDamaged() 추가
    }

    public override void OnDamaged(BaseController attacker, int damage)
    {
        base.OnDamaged(attacker, damage);
    }
}
