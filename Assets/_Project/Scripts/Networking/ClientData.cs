using System;

[Serializable]
public class ClientData
{
    public ulong ClientId { get; }

    public int CharacterId { get; set; } = -1;  // Todo: remove this

    public RaceTemplate Race { get; set; }
    public ClassTemplate Class { get; set; }
    public ArmorTemplate Armor { get; set; }
    public TrinketTemplate Trinket { get; set; }

    public int Team { get; set; } = -1;

    public ClientData(ulong clientId)
    {
        ClientId = clientId;
    }

}