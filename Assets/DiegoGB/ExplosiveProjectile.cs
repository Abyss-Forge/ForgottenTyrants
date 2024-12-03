using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForgottenTyrants;

public abstract class ExplosiveProjectile : Projectile, IDamageable
{
    [Header("Explosive")]
    [SerializeField] protected AnimationCurve _explosionAreaCurve;
    [SerializeField] protected float _explosionRadius = 5;

    [SerializeField] protected bool _proximityEnabled;
    [SerializeField] protected float _proximityDetectionRadius = 5, _proximityExplosionDelay = 1;
    [SerializeField] protected string[] _proximityTriggerTags;

    [SerializeField] protected bool _explodeOnLifetimeEnd;

    [SerializeField] protected bool _isDamageable;
    [SerializeField] public int Health { get; private set; } = 10;
    [SerializeField] protected bool _explodeOnDestroy;

    protected GameObject _directHit;
    protected bool _isExploding;

    void LateUpdate()
    {
        if (_proximityEnabled) PerformProximityCheck(_proximityDetectionRadius);
    }

    public bool PerformProximityCheck(float detectionRadius)
    {
        bool hasHit = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            foreach (string tag in _proximityTriggerTags)
            {
                if (hitCollider.gameObject.CompareTag(tag))
                {
                    float effectPercentage = CalculateDistanceBasedEffect(hitCollider.transform.position);

                    if (_hasDamage)
                    {
                        float damage = _damage * effectPercentage;
                        Debug.Log($"Enemy takes {damage} damage");  // TODO
                    }
                    if (_hasKnockback)
                    {
                        float knockback = _knockbackForce * effectPercentage;
                        hitCollider.gameObject.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(knockback, transform.position, _explosionRadius, 3f);
                    }

                    hasHit = true;
                    break;
                }
            }
        }

        return hasHit;
    }

    private float CalculateDistanceBasedEffect(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        float normalizedDistance = 1 - Mathf.Clamp01(distance / _explosionRadius);
        return _explosionAreaCurve.Evaluate(normalizedDistance);
    }

    #region Damageable

    public event Action OnDeath;
    public event Action<int> OnDamage, OnHeal;

    public void Damage(int damageAmount)
    {
        if (!_isDamageable) return;

        Health -= damageAmount;
        OnDamage?.Invoke(damageAmount);

        if (Health <= 0) OnDeath?.Invoke();
    }

    public void Heal(int healAmount)
    {
        if (!_isDamageable) return;

        Health += healAmount;
        OnHeal?.Invoke(healAmount);
    }

    #endregion

}