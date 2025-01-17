using UnityEngine;

[RequireComponent(typeof(DamageableBehaviour))]
public class DestroyableBehaviour : MonoBehaviour
{
    DamageableBehaviour _damageable;

    [SerializeField] private bool _destroyOnDeath;

    void Awake()
    {
        _damageable = GetComponentInParent<DamageableBehaviour>();
    }

    void OnEnable()
    {
        _damageable.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        _damageable.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (_destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}