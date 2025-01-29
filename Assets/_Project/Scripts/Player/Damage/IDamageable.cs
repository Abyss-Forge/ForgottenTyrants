using System;

public interface IDamageable
{
    // Property to get and set the health of the object.
    int Health { get; }

    // Method to apply damage to the object.
    void Damage(int damageAmount);

    // Optional: Method to heal the object.
    void Heal(int healAmount);

    // Optional: Event to notify when the object's health reaches 0.
    event Action OnDeath;

    // Optional: Event to notify whenever the object takes damage.
    event Action<int> OnDamage, OnHeal;
}