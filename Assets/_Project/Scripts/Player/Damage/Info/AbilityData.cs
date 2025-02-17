using System;
using Unity.Netcode;

public enum EDamageApplyChannel
{
    MYSELF = 0,
    ALLIES = 1,
    ENEMIES = 2,
}

public interface IAbilityData : INetworkSerializable
{
    public AbilityData AbilityData { get; }
}

//abstract class
public struct AbilityData : INetworkSerializable, IEquatable<AbilityData>
{
    int _hash;
    public int Hash => _hash;

    ulong _playerId;
    public ulong PlayerId => _playerId;

    int _teamId;
    public int TeamId => _teamId;

    EDamageApplyChannel _affectedChannel;
    public EDamageApplyChannel AffectedChannel => _affectedChannel;

    public AbilityData(ulong playerId, int teamId, EDamageApplyChannel affectedChannel)
    {
        Random random = new Random();
        int randomValue = random.Next();
        _hash = HashCode.Combine(int.MinValue, int.MaxValue, randomValue);

        _playerId = playerId;
        _teamId = teamId;
        _affectedChannel = affectedChannel;
    }

    public bool CanApply(ClientData data)
    {
        if (_affectedChannel == EDamageApplyChannel.MYSELF) return data.ClientId == _playerId;
        if (_affectedChannel == EDamageApplyChannel.ALLIES) return data.TeamId == _teamId;
        if (_affectedChannel == EDamageApplyChannel.ENEMIES) return data.TeamId != _teamId;
        return false;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _playerId);
        serializer.SerializeValue(ref _teamId);
        serializer.SerializeValue(ref _affectedChannel);
    }

    public bool Equals(AbilityData other)
    {
        return //other != null &&
               _playerId == other.PlayerId &&
               _teamId == other.TeamId &&
               _affectedChannel == other.AffectedChannel;
    }
}