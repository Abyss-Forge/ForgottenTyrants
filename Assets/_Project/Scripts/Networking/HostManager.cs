using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using Mono.CSharp;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostManager : Singleton<HostManager>
{
    [SerializeField] private int _maxConnections = 6;
    public int MaxConnections => _maxConnections;

    [SerializeField] private SceneReference _gameplayScene; //TODO remove test

    public Dictionary<ulong, ClientData> ClientDataDict { get; private set; } = new();
    public string JoinCode { get; private set; }

    bool AmIHost;

    public RaceTemplate Race;
    public ClassTemplate Class;
    public ArmorTemplate Armor;
    public TrinketTemplate Trinket;

    private bool _isGameStarted;
    private string _lobbyId;

    public ClientData GetMyClientData()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton is null.");
            return null;
        }

        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.LogError("Client is not connected yet.");
            //return null;
        }

        if (ClientDataDict == null)
        {
            Debug.LogError("ClientData dictionary is null.");
            ClientDataDict = new();
            //return null;
        }

        if (!AmIHost)
        {
            ulong id = NetworkManager.Singleton.LocalClientId;
            if (!ClientDataDict.TryGetValue(id, out ClientData clientData))
            {
                // Si no existe, se crea y se agrega
                clientData = new ClientData(id);
                clientData.Race = Race;
                clientData.Class = Class;
                clientData.Armor = Armor;
                clientData.Trinket = Trinket;
                ClientDataDict[id] = clientData;
                Debug.Log("la seleccion de clase ES MENTIRA");
            }
        }

        if (!ClientDataDict.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogError($"ClientData does not contain LocalClientId: {NetworkManager.Singleton.LocalClientId}");
            return null;
        }

        if (ClientDataDict[NetworkManager.Singleton.LocalClientId] == null)
        {
            Debug.LogError($"esto no se deberia ver nunca: {NetworkManager.Singleton.LocalClientId}");
            return null;
        }

        Debug.Log("im host: " + AmIHost);
        return ClientDataDict[NetworkManager.Singleton.LocalClientId];
    }

    public async Task StartHost(bool isPrivate = false, string lobbyName = null)
    {
        AmIHost = true;

        Allocation allocation;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(_maxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError($"Relay create allocation request failed {e.Message}");
            throw;
        }

        Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"server: {allocation.AllocationId}");

        try
        {
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch
        {
            Debug.LogError("Relay get join code request failed");
            throw;
        }

        var relayServerData = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        try
        {
            var createLobbyOptions = new CreateLobbyOptions();
            createLobbyOptions.IsPrivate = isPrivate;
            createLobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: JoinCode
                    )
                }
            };

            lobbyName ??= $"Room {JoinCode}";
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, _maxConnections, createLobbyOptions);
            _lobbyId = lobby.Id;
            StartCoroutine(HeartbeatLobbyCoroutine(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            throw;
        }

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        // ClientDataDict = new();
        NetworkManager.Singleton.StartHost();
    }

    private IEnumerator HeartbeatLobbyCoroutine(float waitTimeSeconds)
    {
        var delay = new WaitForSeconds(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
            yield return delay;
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (ClientDataDict.Count >= _maxConnections || _isGameStarted)
        {
            response.Approved = false;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;

        ClientDataDict[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);

        Debug.Log($"Client {request.ClientNetworkId} has connected");
    }

    private void OnNetworkReady()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        SceneLoaderWrapper.Instance.LoadScene(ForgottenTyrants.Scene.CharacterSelect, useNetworkSceneManager: true, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (ClientDataDict.ContainsKey(clientId))
        {
            if (ClientDataDict.Remove(clientId))
            {
                Debug.Log($"Client {clientId} has disconnected");
            }
        }
    }

    public void SetCharacterBuild(ulong clientId, RaceTemplate race, ClassTemplate @class, ArmorTemplate armor, TrinketTemplate trinket)
    {
        Debug.Log("build");
        if (ClientDataDict.TryGetValue(clientId, out ClientData data))
        {
            data.Race = race;
            data.Class = @class;
            data.Armor = armor;
            data.Trinket = trinket;
            Debug.Log("build");
        }
    }

    public void SetTeam(ulong clientId, int teamId)
    {
        Debug.Log("team");
        if (ClientDataDict.TryGetValue(clientId, out ClientData data))
        {
            data.TeamId = teamId;
            Debug.Log("team");
        }
    }

    public void SetCharacter(ulong clientId, CharacterTemplate character)
    {
        Debug.Log("character");
        if (ClientDataDict.TryGetValue(clientId, out ClientData data))
        {
            data.Character = character;
            Debug.Log("character");
        }
    }

    public void StartGame()
    {
        _isGameStarted = true;

        SceneLoaderWrapper.Instance.LoadScene(_gameplayScene.Name, useNetworkSceneManager: true, LoadSceneMode.Single);
    }

}