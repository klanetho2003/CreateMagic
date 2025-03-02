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

        Collider = gameObject.GetComponent<CircleCollider2D>();
        RigidBody = GetComponent<Rigidbody2D>();

        ObjectType = EObjectType.ProjecTile;
        SpriteRenderer.sortingOrder = SortingLayers.PROJECTILE;

        return true;
    }

    public void SetInfo(int dataTemplateID) // in ObjectManager >> Init ���� ����Ǵ� Init ���� // SkillBase > ObjectManager > ProjectileController
    {
        ProjectileData = Managers.Data.ProjectileDic[dataTemplateID];

        // Name
        gameObject.name = $"{ProjectileData.DataId}_{ProjectileData.Name}";

        if (string.IsNullOrEmpty(ProjectileData.MaterialID) == false)
            SpriteRenderer.material = Managers.Resource.Load<Material>(ProjectileData.MaterialID);

        if (string.IsNullOrEmpty(ProjectileData.AnimatorDataID) == false) 
            Anim.runtimeAnimatorController = Managers.Resource.Load<RuntimeAnimatorController>(ProjectileData.AnimatorDataID);
    }

    public void SetSpawnInfo(CreatureController owner, SkillBase skill, LayerMask layer, Action<BaseController> onHit) // SkillBase ��Generate�Լ� �� �κп� ��ġ,
                                                                                         // ��, SetInfo > Generate�Լ� ���� > SetSpawnInfo
    {
        Owner = owner;
        Skill = skill;
        _onHit = onHit;
        TargetPosition = (owner.Target == null) ? (owner.GenerateSkillPosition - owner.CenterPosition).normalized * PROJECTILE_DISTANCE_MAX : owner.Target.CenterPosition;

        // Size
        Collider.radius = ProjectileData.ProjRange;

        // Rule
        Collider.excludeLayers = layer;

        // Move
        if (ProjectileMotion != null)
            Destroy(ProjectileMotion); // Remove Component when Pooling revive

        // Reserve
        StartCoroutine(CoReserveDestroy(ProjectileData.ReserveDestroyTime));

        // Motion
        string compoenetName = ProjectileData.ComponentName;
        if (compoenetName == null)
            return;

        ProjectileMotion = gameObject.AddComponent(Type.GetType(compoenetName)) as ProjectileMotionBase;

        LinearMotion linearMotion = ProjectileMotion as LinearMotion;
        if (linearMotion != null)
            linearMotion.SetInfo(ProjectileData.DataId, owner.GenerateSkillPosition, TargetPosition, () => Managers.Object.Despawn(this));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BaseController target = other.GetComponent<BaseController>();
        if (target.IsValid() == false)
            return;

        // To Do
        if (_onHit != null)
            _onHit.Invoke(target);
    }

    private IEnumerator CoReserveDestroy(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Managers.Object.Despawn(this);
    }

    protected override void Clear()
    {
        base.Clear();

        /* To Do : ��� Projectile�� ������ Prefab�� ������ �� �ֵ��� �����Ǿ�,
        Pooling�� �� CastingImpact�� ������ scale�� ����Ǵ� ������ �����.
        Pooling ����ȭ�ϰ�, �Ʒ� �ڵ带 ���쵵������*/
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.Euler(0, 0, 0); ;
    }
}
