using UnityEngine;

public enum EStat
{
    HEALTH = 0,
    MOVEMENT_SPEED = 1,
    ATTACK_SPEED = 2,
    COOLDOWN_REDUCTION = 3,
    PHYSIC_DAMAGE = 4,
    MAGIC_DAMAGE = 5,
    PHYSIC_DEFENSE = 6,
    MAGIC_DEFENSE = 7,
}

[System.Serializable]
public class Stats
{
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

    public Stats(Stats stats) : base()
    {
        _health = stats.Health;
        _physicalDamage = stats.PhysicalDamage;
        _magicalDamage = stats.MagicalDamage;
        _physicalDefense = stats.PhysicalDefense;
        _magicalDefense = stats.MagicalDefense;
        _movementSpeed = stats.MovementSpeed;
        _attackSpeed = stats.AttackSpeed;
        _cooldownReduction = stats.CooldownReduction;
    }

    public Stats(int health, int physicalDamage, int magicalDamage, float movementSpeed, float attackSpeed, int physicalDefense, int magicalDefense, float cooldownReduction) : base()
    {
        _health = health;
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

    public ref float Get(EStat stat)
    {
        switch (stat)
        {
            case EStat.HEALTH:
                return ref _health;
            case EStat.MOVEMENT_SPEED:
                return ref _movementSpeed;
            case EStat.ATTACK_SPEED:
                return ref _attackSpeed;
            case EStat.COOLDOWN_REDUCTION:
                return ref _cooldownReduction;
            case EStat.PHYSIC_DAMAGE:
                return ref _physicalDamage;
            case EStat.MAGIC_DAMAGE:
                return ref _magicalDamage;
            case EStat.PHYSIC_DEFENSE:
                return ref _physicalDefense;
            case EStat.MAGIC_DEFENSE:
                return ref _magicalDefense;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(stat), $"Stat {stat} is not defined.");
        }
    }


    public void Set(EStat stat, float value)
    {
        switch (stat)
        {
            case EStat.HEALTH:
                _health = value;
                break;
            case EStat.MOVEMENT_SPEED:
                _movementSpeed = value;
                break;
            case EStat.ATTACK_SPEED:
                _attackSpeed = value;
                break;
            case EStat.COOLDOWN_REDUCTION:
                _cooldownReduction = value;
                break;
            case EStat.PHYSIC_DAMAGE:
                _physicalDamage = value;
                break;
            case EStat.MAGIC_DAMAGE:
                _magicalDamage = value;
                break;
            case EStat.PHYSIC_DEFENSE:
                _physicalDefense = value;
                break;
            case EStat.MAGIC_DEFENSE:
                _magicalDefense = value;
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(stat), $"Stat {stat} is not defined.");
        }
    }

    //TODO Transicionarlo a buffableBehaviour
    public void ChangePhysicalDamage(float newPhysicalDamage)
    {
        _physicalDamage = newPhysicalDamage;
    }
    public void ChangeMagicalDamage(float newMagicalDamage)
    {
        _magicalDamage = newMagicalDamage;
    }
}