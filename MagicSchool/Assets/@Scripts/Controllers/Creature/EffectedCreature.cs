using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EffectedCreature : CreatureController
{
    Material m;
    CreatureController _owner; // 데미지를 가한 cc

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        m = GetComponent<Renderer>().material;

        return true;
    }

    #region Burn

    int _burnDamage = 5;

    public bool _isOnBurn { get { return _burnDuration > 0; } }

    int _burnDuration;
    protected int BurnDuration
    {
        get { return _burnDuration; }
        private set
        {
            _burnDuration = value;

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
    #region Burn Material Setting Method
    
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

    #endregion

    protected virtual void OnBurnDamaged()
    {
        // effect
        OnDamaged(_owner, _burnDamage);
    }

    protected override void Clear()
    {
        base.Clear();
        InitBurn();
    }

    float _lastDamageSeconds;
    public float DamageCycle{ get; private set; }

    protected Coroutine _coOnBurn;
    public virtual void OnBurn(CreatureController cc, int addDuration)
    {
        _owner = cc;
        BurnDuration += addDuration;

        if (_coOnBurn == null)
            _coOnBurn = StartCoroutine(CoOnBurn(1));
    }

    public virtual void OnBurn(CreatureController cc, float damageCycle, int addDuration)
    {
        _owner = cc;
        DamageCycle = damageCycle;
        BurnDuration += addDuration;

        if (_coOnBurn == null)
            _coOnBurn = StartCoroutine(CoOnBurn(1));
    }

    protected virtual IEnumerator CoOnBurn(int seconds)
    {
        while (BurnDuration > 0)
        {
            yield return new WaitForSeconds(seconds);
            _lastDamageSeconds += seconds;

            if (_lastDamageSeconds >= DamageCycle)
            {
                OnBurnDamaged();
                // To Do : Effect 추가

                _lastDamageSeconds = 0;
            }

            BurnDuration -= seconds;
        }

        InitBurn();
    }

    void InitBurn() // Burn상태 종료
    {
        _burnDuration = 0;
        OffBurnMaterial();

        DamageCycle = 0;
        _owner = null;
        _coOnBurn = null; 
    }

    #endregion
}
