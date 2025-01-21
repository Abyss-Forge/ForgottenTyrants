using System;
using Unity.Netcode;

public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    public ulong ClientId;
    public bool IsLockedIn;
    public int TeamId;
    public int CharacterId;

    public CharacterSelectState(ulong clientId, int teamId = -1, bool isLockedIn = false, int characterId = -1)
    {
        ClientId = clientId;
        IsLockedIn = isLockedIn;
        TeamId = teamId;
        CharacterId = characterId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref IsLockedIn);
        serializer.SerializeValue(ref TeamId);
        serializer.SerializeValue(ref CharacterId);
    }

    public bool Equals(CharacterSelectState other)
    {
        return ClientId == other.ClientId &&
            IsLockedIn == other.IsLockedIn &&
            TeamId == other.TeamId &&
            CharacterId == other.CharacterId;
    }

}