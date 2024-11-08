using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBehaviour : MonoBehaviour, IDamageable
{
    [field: SerializeField] public int Health { get; private set; }

    public event Action OnDeath;
    public event Action<int> OnTakeDamage;

    public void Damage(int damageAmount)
    {
        Health -= damageAmount;
        OnTakeDamage?.Invoke(damageAmount);

        if (Health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(int healAmount)
    {
        Health += healAmount;
    }
}
