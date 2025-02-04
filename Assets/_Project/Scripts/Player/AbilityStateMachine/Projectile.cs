using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForgottenTyrants;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(InfoContainer))]
public abstract class Projectile : NetworkBehaviour
{
    protected Rigidbody _rigidbody { get; set; }
    protected CapsuleCollider _collider { get; set; }

    public InfoContainer InfoContainer { get; set; }

    public enum EProjectileState
    {
        LIVE, HIT, DESTROYED
    }

    [SerializeField] protected float _lifetime = 5, _gravityMultiplier = 1;

    [Header("Knockback")]
    [SerializeField] protected bool _hasKnockback;
    [SerializeField] protected float _knockbackForce = 500;

    [Header("Damage")]
    [SerializeField] protected bool _hasDamage;
    [SerializeField] protected float _damage = 100;

    [Header("Ricochet")]
    [SerializeField] protected bool _canRicochet;
    [SerializeField] protected int _ricochets = 5;
    [SerializeField, Range(0, 90)] protected float _maxRicochetAngle = 45;

    protected float _lifetimeTimer;
    protected int _remainingRicochets;

    public override void OnNetworkSpawn()
    {
        //Awake();
    }

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        InfoContainer = GetComponent<InfoContainer>();

        _lifetimeTimer = _lifetime;
        _remainingRicochets = _ricochets;
    }

    protected virtual void Update()
    {
        if (_lifetimeTimer > 0) _lifetimeTimer -= Time.deltaTime;
        else OnLifetimeEnd();
    }

    protected virtual void FixedUpdate()
    {
        if (_rigidbody.useGravity) _rigidbody.AddForce(Physics.gravity * (_gravityMultiplier - 1f), ForceMode.Acceleration);
    }   // Al multiplicador se le resta 1 porque  por defecto la gravedad ya se aplica una vez al tener rigidbody

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(Tag.Enemy))
        {
            OnHit();
        }
        else if (_canRicochet && _remainingRicochets > 0)
        {
            Vector3 collisionNormal = other.contacts[0].normal;
            Vector3 incomingDirection = _rigidbody.velocity.normalized;
            float angle = Vector3.Angle(incomingDirection, -collisionNormal) - 90;

            if (angle <= _maxRicochetAngle)
            {
                _remainingRicochets--;
                return;
                //  Vector3 reflectedDirection = Vector3.Reflect(incomingDirection, collisionNormal);
                //  _rigidbody.velocity = reflectedDirection * _rigidbody.velocity.magnitude;
            }
        }

        OnHit();
    }


    protected virtual void OnHit()
    {
        //vfx & sound
        Debug.Log("Hit");
        Destroy(gameObject);
    }

    protected virtual void OnLifetimeEnd()
    {
        OnHit();
    }

}