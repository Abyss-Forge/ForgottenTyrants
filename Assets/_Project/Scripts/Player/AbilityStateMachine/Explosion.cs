using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(InfoContainer))]
public class Explosion : MonoBehaviour
{
    SphereCollider _collider { get; set; }
    public InfoContainer InfoContainer { get; protected set; }

    [SerializeField] private float _radius;
    [SerializeField] private float _propagationTime;
    [Tooltip("Used for speed, damage or knockback at a certain point of the explosion")]
    [SerializeField] private AnimationCurve _propagationCurve;

    void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        InfoContainer = GetComponent<InfoContainer>();

        _collider.radius = 0.1f;
        StartCoroutine(EnlargeCollider());
    }

    private IEnumerator EnlargeCollider()
    {
        float elapsedTime = 0f;
        float initialRadius = _collider.radius;

        while (elapsedTime < _propagationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _propagationTime;
            _collider.radius = Mathf.Lerp(initialRadius, _radius, t);

            InfoContainer.SetMultiplier(CalculateDistanceBasedEffect(initialRadius, _radius));
            yield return null;
        }

        _collider.radius = _radius;
    }

    private float CalculateDistanceBasedEffect(float min, float max)
    {
        float distance = max - min; //TODO revisar calculo con chatgpt
        float normalizedDistance = 1 - Mathf.Clamp01(distance / _radius);
        return _propagationCurve.Evaluate(normalizedDistance);
    }

}