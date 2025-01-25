using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public CircleCollider2D Collider { get; private set; }
    public Rigidbody2D RigidBody { get; private set; }
    public Animator Anim { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    public float ColliderRadius { get { return (Collider != null) ? Collider.radius : 0.0f; } }
    public Vector3 CenterPosition { get { return transform.position + Vector3.up * ColliderRadius; } }

    public int DataTemplateID { get; set; }

    #region Init
    
    void Awake()
    {
        Init();
    }

    bool _init = false;

    public virtual bool Init() // 최초 실행일 떄는 true를 반환, 한 번이라도 실행한 내역이 있을 경우 false를 반환
    {
        if (_init)
            return false;

        Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
        Anim = GetComponent<Animator>();
        RigidBody = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();

        _init = true;
        return true;
    }

    #endregion

    #region Update
    
    void Update()
    {
        UpdateController();
    }

    public virtual void UpdateController() { }

    #endregion

    #region FixedUpdate
    
    void FixedUpdate()
    {
        FixedUpdateController();
    }

    public virtual void FixedUpdateController() { }

    #endregion

    #region Battle

    public virtual void OnDamaged(BaseController attacker, int damage) { }

    protected virtual void OnDead() { }

    #endregion

    public void OnAnimationEvent(string eventName)
    {
        AnimationEventManager.OnAnimationEvent(this, eventName);
    }

    protected virtual void Clear()
    {

    }
}
