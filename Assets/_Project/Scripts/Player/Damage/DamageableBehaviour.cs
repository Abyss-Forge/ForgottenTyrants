using System;
using UnityEngine;

public class DamageableBehaviour : MonoBehaviour, IDamageable, IInitializable
{
    [Tooltip("Leave at 0 to initialize from code")]
    [SerializeField] private int _health;
    public int Health => _health;
    public event Action OnDeath;
    public event Action<int> OnDamage, OnHeal;

    [SerializeField] private bool _canBeReinitialized;
    private bool _isInitialized;
    public bool IsInitialized => _isInitialized;

    public void Initialize() => Initialize(_health);
    public void Initialize(int health)
    {
        if (_isInitialized && !_canBeReinitialized) return;
        _isInitialized = true;

        _health = health;
    }

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        OnDamage?.Invoke(damageAmount);

        if (_health <= 0) OnDeath?.Invoke();
    }

    public void Heal(int healAmount)
    {
        _health += healAmount;
        OnHeal?.Invoke(healAmount);
    }

}