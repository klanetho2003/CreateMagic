using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObjController : MonoBehaviour
{
    //public SkillObjController() : base(Define.ESkillType.None) { } // ���� SKillBase�� �������� �б� ���� SKillBase�� ��� ���� ���̱⿡ type�� Mone���� �־����


    public float _lifeTime { get; protected set; }

    public CreatureController _owner { get; protected set; }

    public  Action<CreatureController> _OnHit{ get; protected set; }


    public virtual void InitValue()
    {
        // StartDestory(this, _lifeTime); // temp
    }
}

