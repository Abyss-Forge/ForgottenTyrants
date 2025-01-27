using System.Collections.Generic;

public enum EElementalType
{
    PHYSIC, MAGIC
}

public class DamageInfo //: AbilityInfo
{
    public float DamageAmount { get; set; }
    public EElementalType DamageType { get; }

    public DamageInfo(ulong playerId, int teamId, EDamageApplyChannel affectedChannel, float damageAmount, EElementalType damageType) //: base(playerId, teamId, affectedChannel)
    {
        DamageAmount = damageAmount;
        DamageType = damageType;
    }
}

public class HealInfo //: AbilityInfo
{
    public float HealAmount { get; }

    public HealInfo(ulong playerId, int teamId, EDamageApplyChannel affectedChannel, float healAmount) //: base(playerId, teamId, affectedChannel)
    {
        HealAmount = healAmount;
    }
}

public class BuffInfo //: AbilityInfo
{
    public EStat Stat { get; }
    public float Value { get; }
    public bool IsDebuff { get; }
    public bool IsPercentual { get; }
    public float Duration { get; }

    public BuffInfo(ulong playerId, int teamId, EDamageApplyChannel affectedChannel, EStat stat, float value, bool isDebuff = false, bool isPercentual = false, float duration = -1) //: base(playerId, teamId, affectedChannel)
    {
        Stat = stat;
        Value = value;
        IsDebuff = isDebuff;
        IsPercentual = isPercentual;
        Duration = duration;
    }
}