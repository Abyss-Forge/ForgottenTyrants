using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Utils.Extensions;

[RequireComponent(typeof(SphereCollider))]
public class Explosion : MonoBehaviour
{
    SphereCollider _collider { get; set; }
    InfoContainer _infoContainer { get; set; }

    [Header("References")]
    [SerializeField] private ParticleSystem _vfx;
    [SerializeField] private AudioSource _sfx;

    [Header("Settings")]
    [SerializeField] private float _radius = 3;
    [SerializeField] private float _propagationTime = 1;
    [Tooltip("Used for speed, damage or knockback at a certain point of the explosion")]
    [SerializeField] private AnimationCurve _propagationCurve;

    [Header("Knockback")]
    [SerializeField] private bool _hasKnockback;
    [SerializeField] private float _knockbackForce = 500;

    private float _initialRadius;

    void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _infoContainer = GetComponentInParent<InfoContainer>();

        _initialRadius = _collider.radius;
        _collider.enabled = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (!_hasKnockback) return;

        other.gameObject.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(_knockbackForce, transform.position, _radius, 3f);
    }

    public Task Explode()
    {
        Task explosion = EnlargeColliderAsync();
        Task sfx = _sfx.PlayAndAwaitFinish();
        Task vfx = _vfx.PlayAndAwaitFinish();
        return Task.WhenAll(explosion, vfx, sfx);
    }

    private async Task EnlargeColliderAsync()
    {
        _collider.enabled = true;
        _collider.radius = _initialRadius;

        float elapsedTime = 0f;
        while (elapsedTime < _propagationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _propagationTime;
            _collider.radius = Mathf.Lerp(_initialRadius, _radius, t);

            _infoContainer.SetMultiplier(CalculateDistanceBasedEffect(_collider.radius));
            await Task.Yield();
        }

        _collider.radius = _radius;
        await Task.Yield();
        _collider.enabled = false;
    }

    private float CalculateDistanceBasedEffect(float currentRadius)
    {
        float normalizedDistance = 1 - Mathf.Clamp01(currentRadius / _radius);
        return _propagationCurve.Evaluate(normalizedDistance);
    }

}