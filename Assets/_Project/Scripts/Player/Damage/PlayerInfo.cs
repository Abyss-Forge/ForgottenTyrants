using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public ClientData ClientData { get; private set; }
    public Stats Stats { get; private set; }

    void Awake()
    {
        ClientData = HostManager.Instance.GetMyClientData();
        InitializeBaseBuildData(ClientData);
    }

    private void InitializeBaseBuildData(ClientData data)
    {
        Stats = new();
        Stats.Add(data.Race.Stats);
        Stats.Add(data.Class.Stats);
        Stats.Add(data.Armor.Stats);
        Stats.Add(data.Trinket.Stats);
    }

}