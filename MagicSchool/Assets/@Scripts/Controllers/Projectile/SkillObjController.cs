using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObjController : MonoBehaviour
{
    //public SkillObjController() : base(Define.ESkillType.None) { } // 그저 SKillBase의 정보만을 읽기 위해 SKillBase를 상속 받은 것이기에 type을 Mone으로 넣어줬다


    public float _lifeTime { get; protected set; }

    public CreatureController _owner { get; protected set; }

    public  Action<CreatureController> _OnHit{ get; protected set; }


    public virtual void InitValue()
    {
        // StartDestory(this, _lifeTime); // temp
    }
}

