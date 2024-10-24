using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlly : MonoBehaviour, IDamageable
{
    public int health { get; set; }

    // Declare the events from the IDamageable interface.
    public event Action OnDeath;
    public event Action<int> OnTakeDamage;

    public void Damage(int damageAmount)
    {
        health -= damageAmount;

        // Trigger the OnTakeDamage event if there are subscribers.
        OnTakeDamage?.Invoke(damageAmount);

        if (health <= 0)
        {
            // Trigger the OnDeath event if there are subscribers.
            OnDeath?.Invoke();
        }
    }

    // Optional: Implement the Heal method if needed.
    public void Heal(int healAmount)
    {
        health += healAmount;
    }
}
