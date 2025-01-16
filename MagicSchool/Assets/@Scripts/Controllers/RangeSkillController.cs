using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSkillController : SkillBase
{
    public Coroutine Co;

    float _lifeTime;
    Vector3 _size =Vector3.one;
    CreatureController _owner;
    Action<CreatureController> _afterTrigger;

    public RangeSkillController() : base(Define.SkillType.None) { } // 그저 SKillBase의 정보만을 읽기 위해 SKillBase를 상속 받은 것이기에 type을 Mone으로 넣어줬다

    public override bool Init()
    {
        base.Init();

        return true;
    }

    public void SetInfo(Data.SkillData skillData, CreatureController owner, float lifTime, Vector3 size, Action<CreatureController> afterTrigger = null)
    {
        if (skillData == null)
        {
            Debug.LogError("RangeSkillController SetInfo Failed");
            return;
        }

        // pooling되면서 값을 초기화하지 못하고 꺼진다
        _size = size;
        _owner = owner;
        _lifeTime = lifTime;
        _afterTrigger = afterTrigger;
        SkillData = skillData;
        // ToDo : Data Paring

        InitValue();
    }

    public void InitValue()
    {
        transform.localScale = _size;
        StartDestory(this, _lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CreatureController cc = collision.GetComponent<CreatureController>();
        _afterTrigger.Invoke(cc);
    }
}

