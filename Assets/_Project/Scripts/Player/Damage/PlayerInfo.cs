using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    private Stats _baseStats = new(), _modifiedStats = new();
    public Stats ModifiedStats => _modifiedStats;

    void Awake()
    {
        InitializeBasicBuildData();


    }

    private void InitializeBasicBuildData()
    {
        ClientData clientData = HostManager.Instance.GetMyClientData();

        _baseStats.Add(clientData.Race.Stats);
        _baseStats.Add(clientData.Class.Stats);
        _baseStats.Add(clientData.Armor.Stats);
        _baseStats.Add(clientData.Trinket.Stats);
    }

    public static void Buff<EStat>(float buffAmount)
    {

    }
}