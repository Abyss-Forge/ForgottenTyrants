using System;
using Unity.Netcode;

public enum EDamageApplyChannel
{
    MYSELF = 0,
    ALLIES = 1,
    ENEMIES = 2
}

[Serializable]
public struct AbilityInfo : INetworkSerializable, IEquatable<AbilityInfo>
{
    public ulong PlayerId;
    public int TeamId;
    public int AffectedChannel;

    public float DamageAmount;

    public AbilityInfo(int affectedChannel, float damageAmount)
    {
        ClientData data = HostManager.Instance.GetMyClientData();

        PlayerId = data.ClientId;
        TeamId = data.TeamId;
        AffectedChannel = affectedChannel;

        DamageAmount = damageAmount;
    }

    public AbilityInfo(ulong playerId, int teamId, int affectedChannel, float damageAmount)
    {
        PlayerId = playerId;
        TeamId = teamId;
        AffectedChannel = affectedChannel;

        DamageAmount = damageAmount;
    }

    public bool CanApply(ClientData data)
    {
        if (AffectedChannel == (int)EDamageApplyChannel.MYSELF) return data.ClientId == PlayerId;
        if (AffectedChannel == (int)EDamageApplyChannel.ALLIES) return data.TeamId == TeamId;
        if (AffectedChannel == (int)EDamageApplyChannel.ENEMIES) return data.TeamId != TeamId;

        return false;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref TeamId);
        serializer.SerializeValue(ref AffectedChannel);

        serializer.SerializeValue(ref DamageAmount);
    }

    public bool Equals(AbilityInfo other)
    {
        return //other != null &&
               PlayerId == other.PlayerId &&
               TeamId == other.TeamId &&
               AffectedChannel == other.AffectedChannel &&

               DamageAmount == other.DamageAmount;
    }
}