using System;

[Serializable]
public class ClientData
{
    public ulong ClientId { get; }

    public int TeamId { get; set; } = -1;
    public RaceTemplate Race { get; set; }
    public ClassTemplate Class { get; set; }
    public ArmorTemplate Armor { get; set; }
    public TrinketTemplate Trinket { get; set; }

    public CharacterTemplate Character { get; set; } // Todo: remove this

    public ClientData(ulong clientId)
    {
        ClientId = clientId;
    }

    public ClientData(ulong clientId, RaceTemplate race, ClassTemplate @class, ArmorTemplate armor, TrinketTemplate trinket) : this(clientId)
    {
        Race = race;
        Class = @class;
        Armor = armor;
        Trinket = trinket;
    }

}