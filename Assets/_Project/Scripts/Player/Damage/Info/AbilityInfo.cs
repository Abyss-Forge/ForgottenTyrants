using System;
using Unity.Netcode;

public enum EDamageApplyChannel
{
    MYSELF = 0,
    ALLIES = 1,
    ENEMIES = 2
}

//Abstract
public class AbilityInfo : INetworkSerializable, IEquatable<AbilityInfo>
{
    public ulong PlayerId;
    public int TeamId;
    public int AffectedChannel;

    public AbilityInfo() { }

    public AbilityInfo(int affectedChannel)
    {
        ClientData data = HostManager.Instance.GetMyClientData();

        PlayerId = data.ClientId;
        TeamId = data.TeamId;
        AffectedChannel = affectedChannel;
    }

    public AbilityInfo(ulong playerId, int teamId, int affectedChannel)
    {
        PlayerId = playerId;
        TeamId = teamId;
        AffectedChannel = affectedChannel;
    }

    public bool CanApply(ClientData data)
    {
        if (AffectedChannel == (int)EDamageApplyChannel.MYSELF) return data.ClientId == PlayerId;
        if (AffectedChannel == (int)EDamageApplyChannel.ALLIES) return data.TeamId == TeamId;
        if (AffectedChannel == (int)EDamageApplyChannel.ENEMIES) return data.TeamId != TeamId;

        return false;
    }

    public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref TeamId);
        serializer.SerializeValue(ref AffectedChannel);
    }

    public virtual bool Equals(AbilityInfo other)
    {
        return other != null &&
               PlayerId == other.PlayerId &&
               TeamId == other.TeamId &&
               AffectedChannel == other.AffectedChannel;
    }
}