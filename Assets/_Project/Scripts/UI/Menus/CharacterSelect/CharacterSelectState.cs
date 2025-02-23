using System;
using Unity.Netcode;

public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    public ulong ClientId;
    public bool IsLockedIn;
    public int TeamId;
    public ulong RaceId, ClassId, ArmorId, TrinketId;

    public int CharacterId; //TODO remove

    public CharacterSelectState(ulong clientId, int teamId = -1, bool isLockedIn = false, int characterId = -1, ulong raceId = default, ulong classId = default, ulong armorId = default, ulong trinketId = default)
    {
        ClientId = clientId;
        IsLockedIn = isLockedIn;
        TeamId = teamId;

        CharacterId = characterId;//this must remain 4th

        RaceId = raceId;
        ClassId = classId;
        ArmorId = armorId;
        TrinketId = trinketId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref IsLockedIn);
        serializer.SerializeValue(ref TeamId);
        serializer.SerializeValue(ref RaceId);
        serializer.SerializeValue(ref ClassId);
        serializer.SerializeValue(ref ArmorId);
        serializer.SerializeValue(ref TrinketId);
        serializer.SerializeValue(ref CharacterId);
    }

    public bool Equals(CharacterSelectState other)
    {
        return ClientId == other.ClientId &&
            IsLockedIn == other.IsLockedIn &&
            TeamId == other.TeamId &&
            RaceId == other.RaceId &&
            ClassId == other.ClassId &&
            ArmorId == other.ArmorId &&
            TrinketId == other.TrinketId &&
            CharacterId == other.CharacterId;
    }

}