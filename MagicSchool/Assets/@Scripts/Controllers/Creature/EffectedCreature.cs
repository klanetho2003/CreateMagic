using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectedCreature : CreatureController
{
    Material m;
    CreatureController _owner; // 데미지를 가한 cc

    public bool _isOnBurn { get { return _duration > 0; } }

    int _duration;
    protected int Duration
    {
        get { return _duration; }
        private set
        {
            _duration = value;

            switch (_isOnBurn)
            {
                case true:
                    OnBurnMaterial();
                    break;
                case false:
                    OffBurnMaterial();
                    break;
            }
        }
    }

    protected virtual void OnBurnMaterial()
    {
        if (_isOnBurn == false) // 방어 코드하는 게 좋겠지
            return;

        m.EnableKeyword("GLOW_ON");
        m.EnableKeyword("FADE_ON");
        m.EnableKeyword("OUTBASE_ON");
    }

    protected virtual void OffBurnMaterial()
    {
        if (_isOnBurn) // 방어 코드하는 게 좋겠지
            return;

        m.DisableKeyword("GLOW_ON");
        m.DisableKeyword("FADE_ON");
        m.DisableKeyword("OUTBASE_ON");
    }

    float _lastDamageSeconds;
    public float DamageCycle{ get; private set; }
    // To Do : 지속시간 추가

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        m = GetComponent<Renderer>().material;

        return true;
    }

    protected Coroutine _coOnBurn;
    public virtual void OnBurn(CreatureController cc, int addDuration)
    {
        _owner = cc;
        Duration += addDuration;

        if (_coOnBurn == null)
            _coOnBurn = StartCoroutine(CoOnBurn(1));
    }

    public virtual void OnBurn(CreatureController cc, float damageCycle, int addDuration)
    {
        _owner = cc;
        DamageCycle = damageCycle;
        Duration += addDuration;

        if (_coOnBurn == null)
            _coOnBurn = StartCoroutine(CoOnBurn(1));
    }

    protected virtual IEnumerator CoOnBurn(int seconds)
    {
        while (Duration > 0)
        {
            yield return new WaitForSeconds(seconds);
            _lastDamageSeconds += seconds;

            if (_lastDamageSeconds >= DamageCycle)
            {
                OnDamaged(_owner, 5); Debug.Log($"Burn {5}");
                // To Do : Effect 추가

                _lastDamageSeconds = 0;
            }

            Duration -= seconds;
        }

        _duration = 0;
        DamageCycle = 0;
        _owner = null;
        _coOnBurn = null; // Burn상태 종료
    }
}
