using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public struct ExplosionData
{
    public float Radius;
    public float PropagationTime;
    [Tooltip("Used for speed, damage or knockback at a certain point of the explosion")]
    public AnimationCurve Propagation;
}

public abstract class ExplosiveProjectile : Projectile, IDamageable
{
    [Header("Explosive")]
    [SerializeField] protected ExplosionData _explosion;
    [SerializeField] protected bool _explodeOnContact, _explodeOnLifetimeEnd;
    [SerializeField] protected string[] _explosionAffectedTags; //TODO [Tag]

    [Header("Proximity")]
    [SerializeField] protected bool _isProximityEnabled;
    [SerializeField] protected BodyPart _proximityDetector;
    [SerializeField] protected float _detonationDelaySeconds = 1;

    [Header("Damageable")]
    [SerializeField] protected bool _isDamageable;
    [field: SerializeField] public int Health { get; protected set; } = 10;
    [SerializeField] protected bool _explodeOnDestroy;

    private void OnEnable()
    {
        if (_isProximityEnabled) _proximityDetector.OnTriggerEnterEvent += OnProximityTriggered;
    }

    private void OnDisable()
    {
        if (_isProximityEnabled) _proximityDetector.OnTriggerEnterEvent -= OnProximityTriggered;
    }

    private async void OnProximityTriggered(Collider other)
    {
        if (_fsm.CurrentState is not ProjectileLiveState) return;

        Debug.Log("proximity triggered");
        await Task.Delay(TimeSpan.FromSeconds(_detonationDelaySeconds));
        await OnHit();
    }

    protected override void OnLifetimeEnd()
    {
        if (_explodeOnLifetimeEnd)
        {
            base.OnLifetimeEnd();
        }
        else
        {
            _fsm.TransitionTo(EProjectileState.DESTROYED);
        }
    }

    protected override bool IsDirectHit(GameObject go)
    {
        return base.IsDirectHit(go) && _explodeOnContact;
    }

    protected override async Task OnHit()
    {
        Explode();
        await base.OnHit();
    }


    public void Explode() => Explode(out _, out _);
    public void Explode(out List<GameObject> targetsHit, out float totalDamageDealt)
    {
        targetsHit = new();
        totalDamageDealt = 0;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosion.Radius);

        /*SphereCollider explosion = gameObject.AddComponent<SphereCollider>();
        explosion.radius = _explosion.Radius;*/

        foreach (Collider hitCollider in hitColliders)
        {
            foreach (string tag in _explosionAffectedTags)
            {
                if (hitCollider.gameObject.CompareTag(tag))
                {
                    targetsHit.Add(hitCollider.gameObject);

                    float effectPercentage = CalculateDistanceBasedEffect(hitCollider.transform.position);

                    if (_hasDamage && InfoContainer != null)
                    {
                        foreach (var info in InfoContainer.InfoList.OfType<DamageInfo>())
                        {
                            info.DamageAmount *= effectPercentage;
                        }

                        float damage = _damage * effectPercentage;
                        Debug.Log($"Enemy takes {damage} damage");

                        totalDamageDealt += damage;
                    }
                    if (_hasKnockback)
                    {
                        float knockback = _knockbackForce * effectPercentage;
                        hitCollider.gameObject.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(knockback, transform.position, _explosion.Radius, 3f);
                    }

                    break;
                }
            }
        }
    }

    private float CalculateDistanceBasedEffect(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        float normalizedDistance = 1 - Mathf.Clamp01(distance / _explosion.Radius);
        return _explosion.Propagation.Evaluate(normalizedDistance);
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