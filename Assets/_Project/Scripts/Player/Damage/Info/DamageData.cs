using System;
using Unity.Netcode;

public enum EElementalType
{
    PHYSIC = 0,
    MAGIC = 1,
}

public struct DamageData : IAbilityData
{
    AbilityData _abilityData;
    public AbilityData AbilityData => _abilityData;

    float _damageAmount;
    public float DamageAmount => _damageAmount;

    int _damageType;
    public int DamageType => _damageType;

    public DamageData(ulong playerId, int teamId, EDamageApplyChannel affectedChannel, float damageAmount, EElementalType damageType)
    {
        _abilityData = new(playerId, teamId, affectedChannel);

        _damageAmount = damageAmount;
        _damageType = (int)damageType;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _abilityData);

        serializer.SerializeValue(ref _damageAmount);
        serializer.SerializeValue(ref _damageType);
    }

    public bool Equals(DamageData other)
    {
        return _abilityData.Equals(other.AbilityData) &&
            _damageAmount == other.DamageAmount &&
            _damageType == other.DamageType;
    }
}

public class HealData : IAbilityData
{
    AbilityData _abilityData;
    public AbilityData AbilityData => _abilityData;

    float _healAmount;
    public float HealAmount => _healAmount;

    public HealData(ulong playerId, int teamId, EDamageApplyChannel affectedChannel, float healAmount)
    {
        _abilityData = new(playerId, teamId, affectedChannel);

        _healAmount = healAmount;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _abilityData);

        serializer.SerializeValue(ref _healAmount);
    }

    public bool Equals(HealData other)
    {
        return _abilityData.Equals(other.AbilityData) &&
            _healAmount == other.HealAmount;
    }
}

public class BuffData : IAbilityData
{
    AbilityData _abilityData;
    public AbilityData AbilityData => _abilityData;

    EStat _stat;
    public EStat Stat => _stat;

    float _value;
    public float Value => _value;

    bool _isDebuff;
    public bool IsDebuff => _isDebuff;

    bool _isPercentual;
    public bool IsPercentual => _isPercentual;

    float _duration;
    public float Duration => _duration;

    public BuffData(ulong playerId, int teamId, EDamageApplyChannel affectedChannel, EStat stat, float value,
                    bool isDebuff = false, bool isPercentual = false, float duration = -1)
    {
        _abilityData = new(playerId, teamId, affectedChannel);

        _stat = stat;
        _value = value;
        _isDebuff = isDebuff;
        _isPercentual = isPercentual;
        _duration = duration;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _abilityData);

        serializer.SerializeValue(ref _stat);
        serializer.SerializeValue(ref _value);
        serializer.SerializeValue(ref _isDebuff);
        serializer.SerializeValue(ref _isPercentual);
        serializer.SerializeValue(ref _duration);
    }

    public bool Equals(BuffData other)
    {
        return _abilityData.Equals(other.AbilityData) &&
            _stat == other.Stat &&
            _value == other.Value &&
            _isDebuff == other.IsDebuff &&
            _isPercentual == other.IsPercentual &&
            _duration == other.Duration;
    }
}