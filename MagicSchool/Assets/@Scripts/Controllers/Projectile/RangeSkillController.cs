using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class RangeSkillController : BaseController
{
    // ���� ������ ��ü Skill
    public SkillBase Skill { get; private set; }
    public CreatureController Owner { get; private set; }
    public Action<BaseController> _onHit;
    public Data.RangeSkillData RangeSkillData { get; private set; }
    //public ProjectileMotionBase ProjectileMotion { get; private set; }
    public Vector3 TargetPosition { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.RangeSkill;
        SpriteRenderer.sortingOrder = SortingLayers.PROJECTILE;

        return true;
    }


    public void SetInfo(int dataTemplateID) // in ObjectManager >> Init ���� ����Ǵ� Init ���� // SkillBase > ObjectManager > ProjectileController
    {
        RangeSkillData = Managers.Data.RangeSkillDic[dataTemplateID];

        // Name
        gameObject.name = $"{RangeSkillData.DataId}_{RangeSkillData.Name}";

        if (RangeSkillData.AnimatorDataID != null)
        {
            Anim.runtimeAnimatorController = Managers.Resource.Load<RuntimeAnimatorController>(RangeSkillData.AnimatorDataID);

            if (Anim == null) // To Do : Data�� ���̸� DataManager �� Interface�� Validation�ϴ� �ڵ带 ��
            {
                Debug.LogWarning($"Projectile Anim Missing {RangeSkillData.Name}");
                return;
            }
        }
    }

    public void SetSpawnInfo(CreatureController owner, SkillBase skill, LayerMask layer, Action<BaseController> onHit)
    {
        Owner = owner;
        Skill = skill;
        _onHit = onHit;

        // Rule
        Collider.excludeLayers = layer;

        // Reserve
        StartCoroutine(CoReserveDestroy(1.0f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseController target = other.GetComponent<BaseController>();
        if (target.IsValid() == false)
            return;

        // To Do
        _onHit.Invoke(target);
    }

    private IEnumerator CoReserveDestroy(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Managers.Object.Despawn(this);
    }
}

