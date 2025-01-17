using System;
using UnityEngine;

public class DamageableBehaviour : MonoBehaviour, IDamageable
{
    [field: SerializeField] public int Health { get; private set; }

    public event Action OnDeath;
    public event Action<int> OnDamage, OnHeal;

    public void Damage(int damageAmount)
    {
        Health -= damageAmount;
        OnDamage?.Invoke(damageAmount);

        if (Health <= 0) OnDeath?.Invoke();
    }

    public void Heal(int healAmount)
    {
        Health += healAmount;
        OnHeal?.Invoke(healAmount);
    }

}