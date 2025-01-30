using System;
using Unity.Netcode;

[Serializable]
public struct AbilityInfoTest : INetworkSerializable, IEquatable<AbilityInfoTest>
{
    public ulong PlayerId;
    public int TeamId;
    public int AffectedChannel;

    public float DamageAmount;

    public AbilityInfoTest(int affectedChannel, float damageAmount)
    {
        ClientData data = HostManager.Instance.GetMyClientData();

        PlayerId = data.ClientId;
        TeamId = data.TeamId;
        AffectedChannel = affectedChannel;

        DamageAmount = damageAmount;
    }

    public AbilityInfoTest(ulong playerId, int teamId, int affectedChannel, float damageAmount)
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

    public bool Equals(AbilityInfoTest other)
    {
        return //other != null &&
               PlayerId == other.PlayerId &&
               TeamId == other.TeamId &&
               AffectedChannel == other.AffectedChannel &&

               DamageAmount == other.DamageAmount;
    }
}