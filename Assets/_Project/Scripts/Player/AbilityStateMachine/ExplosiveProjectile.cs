using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForgottenTyrants;
using System.Linq;
using System.ComponentModel;

[RequireComponent(typeof(SphereCollider))]
public abstract class ExplosiveProjectile : Projectile, IDamageable
{
    [Header("Explosive")]
    [SerializeField] protected AnimationCurve _explosionAreaCurve;
    [SerializeField] protected float _explosionRadius = 5;
    [SerializeField] protected string[] _explosionAffectedTags; //TODO [Tag]
    [SerializeField] protected bool _explodeOnLifetimeEnd;

    [Header("Proximity")]
    [SerializeField] protected float _proximityExplosionDelay = 1;

    [Header("Damageable")]
    [SerializeField] protected bool _isDamageable;
    [SerializeField] public int Health { get; private set; } = 10;
    [SerializeField] protected bool _explodeOnDestroy;

    protected GameObject _directHit;

    void OnTriggerEnter(Collider other)
    {
        foreach (string tag in _explosionAffectedTags)
        {
            Debug.Log(tag);
            if (other.gameObject.CompareTag(tag)) OnProximityTriggered();
        }
    }

    private void OnProximityTriggered()
    {
        Debug.Log("sdfsdfs");
        //await Task.Delay((int)(_proximityExplosionDelay * 1000));
        OnHit();
    }

    protected override void OnHit()
    {
        Explode();
        base.OnHit();
    }

    protected override void OnLifetimeEnd()
    {
        if (_explodeOnLifetimeEnd) Explode();
        base.OnHit();
    }

    public void Explode() => Explode(out _, out _);
    public void Explode(out List<GameObject> targetsHit, out float damageDealt)
    {
        targetsHit = new();
        damageDealt = 0;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            foreach (string tag in _explosionAffectedTags)
            {
                if (hitCollider.gameObject.CompareTag(tag))
                {
                    targetsHit.Add(hitCollider.gameObject);

                    float effectPercentage = CalculateDistanceBasedEffect(hitCollider.transform.position);

                    InfoContainer container = gameObject.GetComponent<InfoContainer>(); //ñapa

                    if (_hasDamage && container != null)
                    {
                        foreach (var info in container.InfoList.OfType<DamageInfo>())
                        {
                            info.DamageAmount *= effectPercentage;
                        }

                        float damage = _damage * effectPercentage;
                        Debug.Log($"Enemy takes {damage} damage");

                        damageDealt += damage;
                    }
                    if (_hasKnockback)
                    {
                        float knockback = _knockbackForce * effectPercentage;
                        hitCollider.gameObject.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(knockback, transform.position, _explosionRadius, 3f);
                    }

                    break;
                }
            }
        }
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