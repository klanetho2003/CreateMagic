using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSkillController : SkillObjController
{
    Vector3 _size = Vector3.one;

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

        _size = size;
        _owner = owner;
        _lifeTime = lifTime;
        _afterTrigger = afterTrigger;
        SkillData = skillData;
        // ToDo : Data Paring

        InitValue();
    }

    public override void InitValue()
    {
        base.InitValue();

        transform.localScale = _size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CreatureController cc = collision.GetComponent<CreatureController>();
        _afterTrigger.Invoke(cc);
    }
}

