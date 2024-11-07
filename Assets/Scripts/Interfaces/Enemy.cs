using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    [field: SerializeField]
    public int _health { get; set; }

    // Declare the events from the IDamageable interface.
    public event Action OnDeath;
    public event Action<int> OnTakeDamage;

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;

        // Trigger the OnTakeDamage event if there are subscribers.
        OnTakeDamage?.Invoke(damageAmount);

        if (_health <= 0)
        {
            // Trigger the OnDeath event if there are subscribers.
            OnDeath?.Invoke();
        }
    }

    // Optional: Implement the Heal method if needed.
    public void Heal(int healAmount)
    {
        _health += healAmount;
    }
}
