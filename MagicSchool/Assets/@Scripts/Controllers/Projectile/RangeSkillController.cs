using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSkillController : SkillObjController
{
    Vector3 _size = Vector3.one;


    public void SetInfo(Data.SkillData skillData, CreatureController owner, float lifTime, Vector3 size, Action<CreatureController> OnHit = null)
    {
        if (skillData == null)
        {
            Debug.LogError("RangeSkillController SetInfo Failed");
            return;
        }

        _size = size;
        _owner = owner;
        _lifeTime = lifTime;
        _OnHit = OnHit;
        //SkillData = skillData;
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

        if (cc.IsValid() == false)
            return;
        /*if (this.IsValid() == false) // temp
            return;*/

        _OnHit.Invoke(cc);
    }
}

