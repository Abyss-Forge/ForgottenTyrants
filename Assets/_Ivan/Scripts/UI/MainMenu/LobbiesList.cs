using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private Transform _lobbyItemParent;
    [SerializeField] private LobbyItem _lobbyItemPrefab;
    [SerializeField] private Button _refreshButton, _closeButton;

    private bool _isRefreshing;
    private bool _isJoining;

    void OnEnable()
    {
        RefreshList();

        _refreshButton.onClick.AddListener(RefreshList);
        _closeButton.onClick.AddListener(Close);
    }

    void OnDisable()
    {
        _refreshButton.onClick.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();
    }

    public async void RefreshList()
    {
        if (_isRefreshing) return;

        _isRefreshing = true;

        try
        {
            var options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0")
            };

            var lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in _lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                var lobbyInstance = Instantiate(_lobbyItemPrefab, _lobbyItemParent);
                lobbyInstance.Initialise(this, lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            _isRefreshing = false;
            throw;
        }

        _isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (_isJoining) return;

        _isJoining = true;

        try
        {
            var joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientManager.Instance.StartClient(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            _isJoining = false;
            throw;
        }

        _isJoining = false;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

}