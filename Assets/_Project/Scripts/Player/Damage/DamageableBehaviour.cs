using System;
using UnityEngine;

public class DamageableBehaviour : MonoBehaviour, IDamageable
{
    [Tooltip("Leave at 0 to initialize from code")]
    [SerializeField] private int _health;
    public int Health => _health;

    public event Action OnDeath;
    public event Action<int> OnDamage, OnHeal;

    //bool _isInitialized;

    void Awake()
    {
        if (Health != 0) Initialize(Health);
    }

    public void Initialize(int health)
    {
        //if (_isInitialized) return;
        //_isInitialized = true;

        _health = health;
    }

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        OnDamage?.Invoke(damageAmount);

        if (Health <= 0) OnDeath?.Invoke();
    }

    public void Heal(int healAmount)
    {
        _health += healAmount;
        OnHeal?.Invoke(healAmount);
    }

}