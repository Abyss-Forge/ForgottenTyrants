using System.Collections.Generic;

public abstract class AbilityInfo
{
    public ulong PlayerId { get; }
    public int TeamId { get; }
    public HashSet<EDamageApplyChannel> AffectedChannels { get; }

    public AbilityInfo(HashSet<EDamageApplyChannel> affectedChannels)
    {
        ClientData data = HostManager.Instance.GetMyClientData();

        PlayerId = data.ClientId;
        TeamId = data.TeamId;
        AffectedChannels = affectedChannels;
    }

    public AbilityInfo(int teamId, HashSet<EDamageApplyChannel> affectedChannels)
    {
        TeamId = teamId;
        AffectedChannels = affectedChannels;
    }

    public AbilityInfo(ulong playerId, int teamId, HashSet<EDamageApplyChannel> affectedChannels) : this(teamId, affectedChannels)
    {
        PlayerId = playerId;
    }

    public bool CanApply(ClientData data)
    {
        if (AffectedChannels.Contains(EDamageApplyChannel.MYSELF))
        {
            return data.ClientId == PlayerId;
        }
        else if (AffectedChannels.Contains(EDamageApplyChannel.ALLIES))
        {
            return data.TeamId == TeamId;
        }
        else if (AffectedChannels.Contains(EDamageApplyChannel.ENEMIES))
        {
            return data.TeamId != TeamId;
        }

        return false;
    }
}