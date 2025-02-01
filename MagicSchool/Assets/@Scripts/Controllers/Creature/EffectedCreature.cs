using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EffectedCreature : CreatureController
{
    CreatureController _owner; // 데미지를 가한 cc
    SkillBase _skill;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    #region Burn

    //int _burnDamage = 5;

    public bool _isOnBurn { get { return _burnDuration > 0; } }

    int _burnDuration;
    protected int BurnDuration
    {
        get { return _burnDuration; }
        private set
        {
            _burnDuration = value;
            SetBurnMaterial();
        }
    }
    #region Burn Material Setting Method
    
    protected virtual void SetBurnMaterial()
    {
        if (_isOnBurn)
        {
            SpriteRenderer.material.EnableKeyword("GLOW_ON");
            SpriteRenderer.material.EnableKeyword("FADE_ON");
            SpriteRenderer.material.EnableKeyword("OUTBASE_ON");
        }
        else
        {
            SpriteRenderer.material.DisableKeyword("GLOW_ON");
            SpriteRenderer.material.DisableKeyword("FADE_ON");
            SpriteRenderer.material.DisableKeyword("OUTBASE_ON");
        }
    }

    #endregion

    protected virtual void OnBurnDamaged(SkillBase skill)
    {
        _skill = skill;
        // effect
        OnDamaged(_owner, skill);
    }

    protected override void Clear()
    {
        base.Clear();
        InitBurn();
    }

    float _lastDamageSeconds;
    public float DamageCycle{ get; private set; }

    protected Coroutine _coOnBurn;
    public virtual void OnBurn(CreatureController cc, int addDuration, SkillBase skill)
    {
        _owner = cc;
        BurnDuration += addDuration;
        _skill = skill;

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
                OnBurnDamaged(_skill);
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
        SetBurnMaterial();

        DamageCycle = 0;
        _owner = null;
        _coOnBurn = null; 
    }

    #endregion
}
