using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [SerializeField] int _hp;
    [SerializeField] int _physicalDamage;
    [SerializeField] int _magicalDamage;
    [SerializeField] int _physicalDefense;
    [SerializeField] int _magicalDefense;
    [SerializeField] float _movementSpeed;
    [SerializeField] float _attackSpeed;
    [SerializeField] float _cooldownReduction;

    public int Hp => _hp;
    public int PhysicalDamage => _physicalDamage;
    public int MagicalDamage => _magicalDamage;
    public int PhysicalDefense => _physicalDefense;
    public int MagicalDefense => _magicalDefense;
    public float MovementSpeed => _movementSpeed;
    public float AttackSpeed => _attackSpeed;
    public float CooldownReduction => _cooldownReduction;

    //TODO [SerializeField] float _range;

    public Stats()
    {
        _hp = 0;
        _physicalDamage = 0;
        _magicalDamage = 0;
        _physicalDefense = 0;
        _magicalDefense = 0;
        _movementSpeed = 0f;
        _attackSpeed = 0f;
        _cooldownReduction = 0f;
    }

    public Stats(int hp, int physicalDamage, int magicalDamage, float movementSpeed, float attackSpeed, int physicalDefense, int magicalDefense, float cooldownReduction)
    {
        _hp = hp;
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
        _hp += other._hp;
        _physicalDamage += other._physicalDamage;
        _magicalDamage += other._magicalDamage;
        _physicalDefense += other._physicalDefense;
        _magicalDefense += other._magicalDefense;
        _movementSpeed += other._movementSpeed;
        _attackSpeed += other._attackSpeed;
        _cooldownReduction += other._cooldownReduction;
    }
}