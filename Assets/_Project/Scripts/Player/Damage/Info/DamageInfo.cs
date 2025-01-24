using System.Collections.Generic;

public class DamageInfo : AbilityInfo
{
    public float DamageAmount { get; set; }
    public EElementalType DamageType { get; }

    public DamageInfo(int teamId, HashSet<EDamageApplyChannel> affectedChannels, float damageAmount, EElementalType damageType) : base(teamId, affectedChannels)
    {
        DamageAmount = damageAmount;
        DamageType = damageType;
    }
}