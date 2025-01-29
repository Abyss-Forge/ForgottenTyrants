using System;
using Unity.Netcode;

public enum EElementalType
{
    PHYSIC, MAGIC
}

public class DamageInfo : AbilityInfo
{
    public float DamageAmount;
    public EElementalType DamageType;

    public DamageInfo(ulong playerId, int teamId, int affectedChannel, float damageAmount, EElementalType damageType) : base(playerId, teamId, affectedChannel)
    {
        DamageAmount = damageAmount;
        DamageType = damageType;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        base.NetworkSerialize(serializer);

        serializer.SerializeValue(ref DamageAmount);
        serializer.SerializeValue(ref DamageType);
    }

    public bool Equals(DamageInfo other)
    {
        return base.Equals(other) &&
               DamageAmount == other.DamageAmount &&
               DamageType == other.DamageType;
    }
}

public class HealInfo : AbilityInfo
{
    public float HealAmount;

    public HealInfo(ulong playerId, int teamId, int affectedChannel, float healAmount) : base(playerId, teamId, affectedChannel)
    {
        HealAmount = healAmount;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        base.NetworkSerialize(serializer);

        serializer.SerializeValue(ref HealAmount);
    }

    public bool Equals(HealInfo other)
    {
        return base.Equals(other) &&
               HealAmount == other.HealAmount;
    }
}

public class BuffInfo : AbilityInfo
{
    public EStat Stat;
    public float Value;
    public bool IsDebuff;
    public bool IsPercentual;
    public float Duration;

    public BuffInfo(ulong playerId, int teamId, int affectedChannel, EStat stat, float value, bool isDebuff = false, bool isPercentual = false, float duration = -1) : base(playerId, teamId, affectedChannel)
    {
        Stat = stat;
        Value = value;
        IsDebuff = isDebuff;
        IsPercentual = isPercentual;
        Duration = duration;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        base.NetworkSerialize(serializer);

        serializer.SerializeValue(ref Stat);
        serializer.SerializeValue(ref Value);
        serializer.SerializeValue(ref IsDebuff);
        serializer.SerializeValue(ref IsPercentual);
        serializer.SerializeValue(ref Duration);
    }

    public bool Equals(BuffInfo other)
    {
        return base.Equals(other) &&
               Stat == other.Stat &&
               Value == other.Value &&
               IsDebuff == other.IsDebuff &&
               IsPercentual == other.IsPercentual &&
               Duration == other.Duration;
    }
}