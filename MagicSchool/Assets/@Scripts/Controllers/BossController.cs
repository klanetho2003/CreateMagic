using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonsterController
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Hp = 1000;

        CreatureState = Define.CreatureState.Skill;

        Skills.AddSkill<Move>(transform.position);
        Skills.AddSkill<Dash>(transform.position); Skills.AddSkill<Dash>(transform.position); Skills.AddSkill<Dash>(transform.position); // ���� ���������� Ʋ���ִ� 3�� �뽬�� ǥ���� ����
        Skills.StartNextSequenceSkill();

        return true;
    }

    public override void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                _animator.Play("Idle");
                break;
            case Define.CreatureState.Moving:
                _animator.Play("Moving");
                break;
            case Define.CreatureState.Skill:
                //_animator.Play("Attack");
                break;
            case Define.CreatureState.Dead:
                _animator.Play("Death");
                break;
        }
    }

    /*protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    float _range = 2.0f;
    protected override void UpdateMoving()
    {
        PlayerController pc = Managers.Game.Player;
        if (pc.IsValid() == false)
            return;

        Vector3 dir = pc.transform.position - transform.position;

        if (dir.magnitude < _range)
        {
            CreatureState = Define.CreatureState.Skill;
            
            // ToDo : MakeDic Ȥ�� Data��Ʈ���� �Ľ�
            foreach (var anim in _animator.runtimeAnimatorController.animationClips)
            {
                if (anim.name == "Boss_01_Attack")
                    Wait(anim.length);
            }
        }
    }

    protected override void UpdateSkill()
    {
        if (_coWait == null)
            CreatureState = Define.CreatureState.Moving;
    }*/

    protected override void UpdateDead()
    {
        if (_coWait == null)
            Managers.Object.Despawn(this);
    }

    protected override void OnDead()
    {
        Skills.StopSkillsAll();

        CreatureState = Define.CreatureState.Dead;

        Wait(3.0f);

        // ToDo : MakeDic Ȥ�� Data��Ʈ���� �Ľ�
        /*foreach (var anim in _animator.runtimeAnimatorController.animationClips)
        {
            if (anim.name == "Boss_01_Death")
                Wait(anim.length);
        }*/
    }

    #region Wait Coroutine
    Coroutine _coWait;

    void Wait(float waitSeconds)
    {
        if (_coWait != null)
        {
            StopCoroutine(_coWait);
            _coWait = null;
        }
            
        _coWait = StartCoroutine(CoWait(waitSeconds));
    }

    IEnumerator CoWait(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        _coWait = null;
    }

    #endregion
}
