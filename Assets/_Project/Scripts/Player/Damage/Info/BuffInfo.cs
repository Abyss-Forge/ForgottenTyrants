using System.Collections.Generic;

public class BuffInfo : NetworkedInfo
{
    public EStat Stat { get; }
    public float Value { get; }
    public bool IsDebuff { get; }
    public bool IsPercentual { get; }
    public float Duration { get; }

    public BuffInfo(int teamId, HashSet<EDamageApplyChannel> affectedChannels, EStat stat, float value, bool isDebuff = false, bool isPercentual = false, float duration = -1) : base(teamId, affectedChannels)
    {
        Stat = stat;
        Value = value;
        IsDebuff = isDebuff;
        IsPercentual = isPercentual;
        Duration = duration;
    }
}