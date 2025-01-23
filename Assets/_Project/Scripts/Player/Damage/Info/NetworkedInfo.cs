using System.Collections.Generic;

public abstract class NetworkedInfo
{
    public ulong PlayerId { get; }
    public int TeamId { get; }
    public HashSet<EDamageApplyChannel> AffectedChannels { get; }

    public NetworkedInfo(HashSet<EDamageApplyChannel> affectedChannels)
    {
        ClientData data = HostManager.Instance.GetMyClientData();

        PlayerId = data.ClientId;
        TeamId = data.TeamId;
        AffectedChannels = affectedChannels;
    }

    public NetworkedInfo(int teamId, HashSet<EDamageApplyChannel> affectedChannels)
    {
        TeamId = teamId;
        AffectedChannels = affectedChannels;
    }

    public NetworkedInfo(ulong playerId, int teamId, HashSet<EDamageApplyChannel> affectedChannels) : this(teamId, affectedChannels)
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