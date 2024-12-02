using System;
using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public abstract class Projectile : MonoBehaviour
{
    protected Rigidbody _rigidbody;
    protected Collider _collider;

    [SerializeField] protected float _lifetime = 5, _gravityMultiplier = 1;

    [Header("Knockback")]
    [SerializeField] protected bool _hasKnockback;
    [SerializeField] protected float _knockbackForce = 500;

    [Header("Damage")]
    [SerializeField] protected bool _hasDamage;
    [SerializeField] protected float _damage = 100;

    [Header("Bounce")]
    [SerializeField] protected bool _canBounce;
    [SerializeField] protected int _bounces = 5;
    [SerializeField, Range(0, 90)] protected float _maxBounceAngle = 45;

    protected float _lifetimeTimer;
    protected int _remainingBounces;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _lifetimeTimer = _lifetime;
        _remainingBounces = _bounces;
    }

    void Update()
    {
        if (_lifetimeTimer > 0)
        {
            _lifetimeTimer -= Time.deltaTime;
        }
        else
        {
            OnLifetimeEnd();
        }
    }

    void FixedUpdate()
    {
        if (_rigidbody.useGravity)
        {
            _rigidbody.AddForce(Physics.gravity * (_gravityMultiplier - 1f), ForceMode.Acceleration);   //-1 porque de base la gravedad ya se aplica una vez
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (_canBounce && _remainingBounces > 0)
        {
            // Calcular el ángulo entre la normal de la superficie y la dirección de movimiento
            Vector3 collisionNormal = other.contacts[0].normal;
            Vector3 incomingDirection = _rigidbody.velocity.normalized;
            float angle = Vector3.Angle(incomingDirection, -collisionNormal) - 90;

            if (angle <= _maxBounceAngle)
            {
                _remainingBounces--;
                return;
                //  Vector3 reflectedDirection = Vector3.Reflect(incomingDirection, collisionNormal);
                //  _rigidbody.velocity = reflectedDirection * _rigidbody.velocity.magnitude;
            }
        }

        OnHit();
    }


    protected virtual void OnHit()
    {
        //
        //vfx & sound
        Destroy(gameObject);
    }

    protected virtual void OnLifetimeEnd()
    {
        OnHit();
    }
}

public class ExplosiveProjectile : Projectile
{
    [Header("Explosive")]
    [SerializeField] protected AnimationCurve _explosionAreaCurve;
    [SerializeField] protected float _explosionRadius = 5;
    [SerializeField] protected bool _proximityEnabled;
    [SerializeField] protected float _proximityDetectionRadius = 5;
    [SerializeField] protected bool _explodeOnLifetimeEnd;

    protected bool _isDirectHit;

}
