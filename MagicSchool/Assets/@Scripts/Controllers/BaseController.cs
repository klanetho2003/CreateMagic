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

    bool _lookLeft = true;
    public bool LookLeft
    {
        get { return _lookLeft; }
        set
        {
            _lookLeft = value;
            FlipX(!value);
        }
    }

    bool _lookDown = true;
    public virtual bool LookDown
    {
        get { return _lookDown; }
        set
        {
            _lookDown = value;
            FlipY(!value);
        }
    }

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

        Collider = gameObject.GetComponent<CircleCollider2D>();
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

    #region Animation

    protected virtual void UpdateAnimation()
    {
    }

    public virtual void SetRigidBodyVelocity(Vector2 velocity)
    {
        if (RigidBody == null)
            return;

        if (velocity.x < 0)
            LookLeft = true;
        else if (velocity.x > 0)
            LookLeft = false;

        if (velocity.y < 0)
            LookDown = true;
        else if (velocity.y > 0)
            LookDown = false;

        RigidBody.velocity = velocity;
    }

    public virtual void FlipX(bool flag)
    {
        if (Anim == null)
            return;

        // On Sprite Flip
        SpriteRenderer.flipX = flag;
    }

    public virtual void FlipY(bool flag)
    {
        if (Anim == null)
            return;

        // override
    }

    public void OnAnimationEvent(string eventName)
    {
        AnimationEventManager.OnAnimationEvent(this, eventName);
    }

    #endregion

    protected virtual void Clear()
    {

    }
}
