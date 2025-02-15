using System;
using UnityEngine;

public class DamageableBehaviour : MonoBehaviour, IDamageable
{
    [Tooltip("Leave at 0 to initialize from code")]
    [SerializeField] private int _health;
    public int Health => _health;

    public event Action OnDeath;
    public event Action<int> OnDamage, OnHeal;

    public void Initialize(int health)
    {
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