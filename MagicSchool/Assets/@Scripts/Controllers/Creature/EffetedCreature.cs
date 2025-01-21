using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffetedCreature : CreatureController
{
    CreatureController _owner; // �������� ���� cc

    float _damageCycle;
    int _burnDamage;
    // To Do : ���ӽð� �߰�

    protected Coroutine _coOnBurn;

    protected virtual void OnBurn(CreatureController cc, float damageCycle)
    {
        _owner = cc;

        if (_coOnBurn == null)
            _coOnBurn = StartCoroutine(CoOnBurn(_damageCycle));

        _damageCycle = damageCycle;
    }

    protected virtual IEnumerator CoOnBurn(float damageCycle)
    {
        while (true)
        {
            OnDamaged(_owner, _burnDamage);
            // To Do : Effect �߰�
            yield return new WaitForSeconds(damageCycle);
        }
        
    }
}
