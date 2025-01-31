using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ProjectileController : BaseController
{
    // ���� ������ ��ü Skill
    public SkillBase Skill { get; private set; }
    public CreatureController Owner { get; private set; }
    public Action<BaseController> _onHit;
    public Data.ProjectileData ProjectileData { get; private set; }
    public ProjectileMotionBase ProjectileMotion { get; private set; }
    public Vector3 TargetPosition { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.ProjecTile;
        SpriteRenderer.sortingOrder = SortingLayers.PROJECTILE;

        return true;
    }

    public void SetInfo(int dataTemplateID) // in ObjectManager >> Init ���� ����Ǵ� Init ���� // SkillBase > ObjectManager > ProjectileController
    {
        ProjectileData = Managers.Data.ProjectileDic[dataTemplateID];

        // Name
        gameObject.name = $"{ProjectileData.DataId}_{ProjectileData.Name}"; // To Do : string data sheet

        // First Sprite
        // SpriteRenderer.sprite

        // Met & Anim // ���� �����ϳ�
        SpriteRenderer.material = Managers.Resource.Load<Material>(ProjectileData.MaterialID);
        Anim.runtimeAnimatorController = Managers.Resource.Load<RuntimeAnimatorController>(ProjectileData.AnimatorDataID);

        if (Anim == null) // To Do : Data�� ���̸� DataManager �� Interface�� Validation�ϴ� �ڵ带 ��
        {
            Debug.LogWarning($"Projectile Anim Missing {ProjectileData.Name}");
            return;
        }
    }

    public void SetSpawnInfo(CreatureController owner, SkillBase skill, LayerMask layer, Action<BaseController> onHit) // SkillBase ��Generate�Լ� �� �κп� ��ġ,
                                                                                         // ��, SetInfo > Generate�Լ� ���� > SetSpawnInfo
    {
        Owner = owner;
        Skill = skill;
        _onHit = onHit;
        TargetPosition = (owner.Target == null) ? (owner.GenerateSkillPosition - owner.transform.position).normalized * PROJECTILE_DISTANCE_MAX : owner.Target.CenterPosition;

        // Rule
        Collider.excludeLayers = layer;

        // Move
        if (ProjectileMotion != null)
            Destroy(ProjectileMotion); // Remove Component when Pooling revive

        string compoenetName = ProjectileData.ComponentName;
        ProjectileMotion = gameObject.AddComponent(Type.GetType(compoenetName)) as ProjectileMotionBase;
        

        LinearMotion linearMotion = ProjectileMotion as LinearMotion;
        if (linearMotion != null)
            linearMotion.SetInfo(ProjectileData.DataId, owner.GenerateSkillPosition, TargetPosition, () => Managers.Object.Despawn(this));

        // Reserve
        StartCoroutine(CoReserveDestroy(5.0f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseController target = other.GetComponent<BaseController>();
        if (target.IsValid() == false)
            return;

        // To Do
        _onHit.Invoke(target);

        Managers.Object.Despawn(this);
    }

    private IEnumerator CoReserveDestroy(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Managers.Object.Despawn(this);
    }
}
