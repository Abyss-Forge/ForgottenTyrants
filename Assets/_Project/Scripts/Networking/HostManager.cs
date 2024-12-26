using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eflatun.SceneReference;
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
    [SerializeField] private SceneReference _characterSelectScene, _gameplayScene;

    private bool _isGameStarted;
    private string _lobbyId;

    public Dictionary<ulong, ClientData> ClientData { get; private set; }
    public string JoinCode { get; private set; }

    public async Task StartHost(bool isPrivate = false, string lobbyName = null)
    {
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

        ClientData = new Dictionary<ulong, ClientData>();

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
        if (ClientData.Count >= _maxConnections || _isGameStarted)
        {
            response.Approved = false;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;

        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);

        Debug.Log($"Added client {request.ClientNetworkId}");
    }

    private void OnNetworkReady()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(_characterSelectScene.Name, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (ClientData.ContainsKey(clientId))
        {
            if (ClientData.Remove(clientId))
            {
                Debug.Log($"Removed client {clientId}");
            }
        }
    }

    public void SetCharacter(ulong clientId, int characterId)
    {
        if (ClientData.TryGetValue(clientId, out ClientData data))
        {
            data.characterId = characterId;
        }
    }

    public void StartGame()
    {
        _isGameStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(_gameplayScene.Name, LoadSceneMode.Single);
    }

}