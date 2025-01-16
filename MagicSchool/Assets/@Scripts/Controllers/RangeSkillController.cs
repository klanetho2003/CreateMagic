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

    public RangeSkillController() : base(Define.SkillType.None) { } // ���� SKillBase�� �������� �б� ���� SKillBase�� ��� ���� ���̱⿡ type�� Mone���� �־����

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

        // pooling�Ǹ鼭 ���� �ʱ�ȭ���� ���ϰ� ������
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

