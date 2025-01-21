using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public enum EStat
    {
        HEALTH, MOVEMENT_SPEED, ATTACK_SPEED, COOLDOWN_REDUCTION,
        PHYSIC_DAMAGE, MAGIC_DAMAGE, PHYSIC_DEFENSE, MAGIC_DEFENSE,
    }

    [SerializeField] float _health;
    [SerializeField] float _physicalDamage;
    [SerializeField] float _magicalDamage;
    [SerializeField] float _physicalDefense;
    [SerializeField] float _magicalDefense;
    [SerializeField] float _movementSpeed;
    [SerializeField] float _attackSpeed;
    [SerializeField] float _cooldownReduction;

    public float Health => _health;
    public float PhysicalDamage => _physicalDamage;
    public float MagicalDamage => _magicalDamage;
    public float PhysicalDefense => _physicalDefense;
    public float MagicalDefense => _magicalDefense;
    public float MovementSpeed => _movementSpeed;
    public float AttackSpeed => _attackSpeed;
    public float CooldownReduction => _cooldownReduction;

    public Stats() { }

    public Stats(int hp, int physicalDamage, int magicalDamage, float movementSpeed, float attackSpeed, int physicalDefense, int magicalDefense, float cooldownReduction) : base()
    {
        _health = hp;
        _physicalDamage = physicalDamage;
        _magicalDamage = magicalDamage;
        _physicalDefense = physicalDefense;
        _magicalDefense = magicalDefense;
        _movementSpeed = movementSpeed;
        _attackSpeed = attackSpeed;
        _cooldownReduction = cooldownReduction;
    }

    public void Add(Stats other)
    {
        _health += other._health;
        _physicalDamage += other._physicalDamage;
        _magicalDamage += other._magicalDamage;
        _physicalDefense += other._physicalDefense;
        _magicalDefense += other._magicalDefense;
        _movementSpeed += other._movementSpeed;
        _attackSpeed += other._attackSpeed;
        _cooldownReduction += other._cooldownReduction;
    }


    //mierdas del diego
    public void ChangePhysicalDamage(float newPhysicalDamage)
    {
        _physicalDamage = newPhysicalDamage;
    }
    public void ChangeMagicalDamage(float newMagicalDamage)
    {
        _magicalDamage = newMagicalDamage;
    }
}