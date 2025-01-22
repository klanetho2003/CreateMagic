using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectedCreature : CreatureController
{
    CreatureController _owner; // �������� ���� cc

    int _duration;
    float _lastDamageSeconds;
    public float DamageCycle{ get; private set; }
    // To Do : ���ӽð� �߰�

    protected Coroutine _coOnBurn;
    public virtual void OnBurn(CreatureController cc, int addDuration)
    {
        _owner = cc;
        _duration += addDuration;

        if (_coOnBurn == null)
            _coOnBurn = StartCoroutine(CoOnBurn(1));
    }

    public virtual void OnBurn(CreatureController cc, float damageCycle, int addDuration)
    {
        _owner = cc;
        DamageCycle = damageCycle;
        _duration += addDuration;

        if (_coOnBurn == null)
            _coOnBurn = StartCoroutine(CoOnBurn(1));
    }

    protected virtual IEnumerator CoOnBurn(int seconds)
    {
        while (_duration > 0)
        {
            yield return new WaitForSeconds(seconds);
            _lastDamageSeconds += seconds;
            _duration -= seconds;

            if (_lastDamageSeconds >= DamageCycle)
            {
                OnDamaged(_owner, 5); Debug.Log($"Burn {5}");
                // To Do : Effect �߰�

                _lastDamageSeconds = 0;
            }
        }
    }
}
