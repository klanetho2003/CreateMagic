using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    float EnvCollectDist { get; set; } = 1.0f;

    [SerializeField]
    Transform _indicator;
    [SerializeField]
    Transform _fireSocket;

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
            if (_moveDir == value.normalized) // �ߺ��� �� �� ������ lastDir�� �ٲ�� ���� ����
                return;

            Vector2 lastDir = _moveDir;
            _moveDir = value.normalized;

            #region �ִϸ��̼� Update
            if (value.x == 0 && value.y == 0)
                return;

            _spriteRenderer.flipX = (value.x == 0) ? lastDir.x > 0 : value.x > 0;
            if (value.y != 0) { _isFront = value.y < 0; }
            UpdateAnimation();
            #endregion
        }
    }

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
                //OnPlayCastingAnimation(); // To Do : Data ��Ʈ
                break;
            case Define.CreatureState.DoSkill:
                _animator.Play($"DoSkill{dir}");
                if (_coWait == null) Wait(0.45f); // ������ �ֵθ��� ��� wait
                break;
            case Define.CreatureState.Dead:
                _animator.Play($"Death{dir}");
                break;
        }
    }

    /*Coroutine _coOnPlayCastingAnimation;
    void OnPlayCastingAnimation()
    {
        if (_coOnPlayCastingAnimation != null)
            StopCoroutine(_coOnPlayCastingAnimation);

        StartCoroutine(CoOnPlayCastingAnimation());
    }

    IEnumerator CoOnPlayCastingAnimation()
    {
        while (true)
        {
            Vector3 initialPosition = transform.localPosition;

            float newY = Mathf.Sin(Time.time) * 0.3f;

            transform.localPosition = new Vector3(initialPosition.x, initialPosition.y + (newY * Time.fixedDeltaTime * 3), initialPosition.z);
            yield return null;
        }
    }*/

    void HandleOnMoveDirChange(Vector2 dir)
    {
        MoveDir = dir;
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _speed = 7.0f;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChange; // ��ü �������� �Բ� �Լ��� �����ϱ⿡ ������ ����
        Managers.Input.OnKeyDownHandler += HandleOnKeyDown;

        Skills = gameObject.GetOrAddComponent<PlayerSkillBook>();

        ObjectType = Define.ObjectType.Player;
        CreatureState = Define.CreatureState.Idle;

        // To Do
        FireBallSkill fireBallSkill = Skills.AddSkill<FireBallSkill>(_indicator.position); //�޾Ƽ� �߰� ���� ����
        Skills.AddSkill<EgoSword>(_indicator.position);

        return true;
    }

    void HandleOnKeyDown(Define.KeyDownEvent key)
    {
        CreatureState = Define.CreatureState.Casting;

        Skills.BuildSKillKey($"{key}");
    }

    private void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChange;
    }

    public override void UpdateController()
    {
        base.UpdateController();
        Debug.Log(CreatureState);
        CollectEnv();
    }

    protected override void UpdateIdle()
    {
        if (_moveDir != Vector2.zero) { CreatureState = Define.CreatureState.Moving; return; }
    }

    protected override void UpdateMoving()
    {
        if (_moveDir == Vector2.zero) { CreatureState = Define.CreatureState.Idle; return; }

        MovePlayer();
    }

    protected override void UpdateCasting()
    {
        MovePlayer();
    }

    protected override void UpdateDoSkill()
    {
        if (_coWait == null)
            Skills.ActiveSkill();
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

    void MovePlayer()
    {
        Vector3 dir = _moveDir * _speed * Time.deltaTime;
        transform.position += dir;

        if (_moveDir != Vector2.zero)
        {
            _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
        }

        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
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

        // To Do : ��⸸ �ص� �ǰ� ������ ������ ���⿡ OnDamaged() �߰�
    }

    public override void OnDamaged(BaseController attacker, int damage)
    {
        base.OnDamaged(attacker, damage);
    }
}
