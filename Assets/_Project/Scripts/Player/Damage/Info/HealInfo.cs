using System.Collections.Generic;

public class HealInfo : AbilityInfo
{
    public float HealAmount { get; }

    public HealInfo(int teamId, HashSet<EDamageApplyChannel> affectedChannels, float healAmount) : base(teamId, affectedChannels)
    {
        HealAmount = healAmount;
    }
}