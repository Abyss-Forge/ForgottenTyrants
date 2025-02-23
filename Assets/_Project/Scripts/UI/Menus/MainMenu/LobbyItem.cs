using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _lobbyNameText, _lobbyPlayersText;
    [SerializeField] private Button _joinButton;

    private LobbyListMenu _lobbiesList;
    private Lobby _lobby;

    void OnEnable()
    {
        _joinButton.onClick.AddListener(Join);
    }

    void OnDisable()
    {
        _joinButton.onClick.RemoveAllListeners();
    }

    public void Initialize(LobbyListMenu lobbiesList, Lobby lobby)
    {
        _lobbiesList = lobbiesList;
        _lobby = lobby;

        _lobbyNameText.text = lobby.Name;
        _lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    private void Join()
    {
        _lobbiesList.JoinAsync(_lobby);
    }

}