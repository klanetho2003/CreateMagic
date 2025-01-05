using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    Vector2 _moveDir = Vector2.zero;

    float EnvCollectDist { get; set; } = 1.0f;

    [SerializeField]
    Transform _indicator;
    [SerializeField]
    Transform _fireSocket;

    public Transform Indicator { get { return _indicator; } }
    public Vector3 FireSocket { get { return _fireSocket.position; } }
    public Vector3 ShootDir { get { return (_fireSocket.position - _indicator.position).normalized; } }

    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set { _moveDir = value.normalized; }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _speed = 5.0f;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChange; // 객체 참조값과 함께 함수를 전달하기에 가능한 구독

        ObjectType = Define.ObjectType.Player;
        CreatureState = Define.CreatureState.Idle;

        // To Do
        FireBallSkill fireBallSkill = Skills.AddSkill<FireBallSkill>(_indicator.position); //받아서 추가 수정 가능

        return true;
    }

    private void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChange;
    }

    void HandleOnMoveDirChange(Vector2 dir)
    {
        _moveDir = dir;

        if (dir.x == 0)
            return;
        _spriteRenderer.flipX = dir.x > 0;
    }

    public override void UpdateController()
    {
        base.UpdateController();
        
        CollectEnv();

        // TEMP
        MovePlayer();
    }

    public override void UpdateAnimation()
    {
        
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

    protected override void UpdateSkill()
    {
        
    }

    void MovePlayer()
    {
        //_moveDir = Managers.Game.MoveDir; // CallBack으로 대체

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
        if (target == null)
            return;

        // To Do : 닿기만 해도 피격 판정이 있으면 여기에 OnDamaged() 추가
    }

    public override void OnDamaged(BaseController attacker, int damage)
    {
        base.OnDamaged(attacker, damage);
    }
}
