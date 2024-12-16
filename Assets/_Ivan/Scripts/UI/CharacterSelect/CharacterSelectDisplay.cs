using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private Transform _charactersHolder, _cardsHolder, _characterInfoPanel, _introSpawnPoint;
    [SerializeField] private CharacterSelectButton _selectButtonPrefab;
    [SerializeField] private PlayerCard _playerCardPrefab;
    [SerializeField] private TMP_Text _characterNameText, _joinCodeText;
    [SerializeField] private Button _lockInButton;

    private GameObject _introInstance;
    private List<CharacterSelectButton> _characterButtons = new();
    private NetworkList<CharacterSelectState> _players;
    private List<PlayerCard> _playerCards;

    void Awake()
    {
        _players = new();
        _playerCards = new();
    }

    void OnEnable()
    {
        _lockInButton.onClick.AddListener(LockIn);
    }

    void OnDisable()
    {
        _lockInButton.onClick.RemoveAllListeners();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            foreach (var character in _characterDatabase.Characters)
            {
                var selectbuttonInstance = Instantiate(_selectButtonPrefab, _charactersHolder);
                selectbuttonInstance.SetCharacter(this, character);
                _characterButtons.Add(selectbuttonInstance);
            }

            _players.OnListChanged += HandlePlayersStateChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }

        if (IsHost)
        {
            _joinCodeText.text = HostManager.Instance.JoinCode;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _players.OnListChanged -= HandlePlayersStateChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        _players.Add(new CharacterSelectState(clientId));

        PlayerCard instance = Instantiate(_playerCardPrefab, _cardsHolder);
        _playerCards.Add(instance);
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != clientId) { continue; }

            _players.RemoveAt(i);
            break;
        }
    }

    public void Select(CharacterTemplate character)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (_players[i].IsLockedIn) { return; }

            if (_players[i].CharacterId == character.ID) { return; }

            if (IsCharacterTaken(character.ID, false)) { return; }
        }

        _characterNameText.text = character.DisplayName;

        _characterInfoPanel.gameObject.SetActive(true);

        if (_introInstance != null)
        {
            Destroy(_introInstance);
        }

        // TODO test //character.IntroAnimationControllerTemp == null
        if (character.IntroAnimationControllerTemp == null)
        {
            _introInstance = Instantiate(character.IntroPrefab, _introSpawnPoint.position, _introSpawnPoint.rotation);
        }
        else
        {
            character.PlayerModelTemp.runtimeAnimatorController = character.IntroAnimationControllerTemp;
            _introInstance = Instantiate(character.IntroPrefab, _introSpawnPoint.position, _introSpawnPoint.rotation);
            character.PlayerModelTemp.runtimeAnimatorController = null;
        }

        SelectServerRpc(character.ID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!_characterDatabase.IsValidCharacterId(characterId)) { return; }

            if (IsCharacterTaken(characterId, true)) { return; }

            _players[i] = new CharacterSelectState(
                _players[i].ClientId,
                characterId,
                _players[i].IsLockedIn
            );
        }
    }

    public void LockIn()
    {
        LockInServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!_characterDatabase.IsValidCharacterId(_players[i].CharacterId)) { return; }

            if (IsCharacterTaken(_players[i].CharacterId, true)) { return; }

            _players[i] = new CharacterSelectState(
                _players[i].ClientId,
                _players[i].CharacterId,
                true
            );
        }

        foreach (var player in _players)
        {
            if (!player.IsLockedIn) { return; }
        }

        foreach (var player in _players)
        {
            HostManager.Instance.SetCharacter(player.ClientId, player.CharacterId);
        }

        HostManager.Instance.StartGame();
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (int i = 0; i < _playerCards.Count; i++)
        {
            if (_players.Count > i)
            {
                _playerCards[i].UpdateDisplay(_players[i]);
            }
            else
            {
                _playerCards[i].DisableDisplay();
            }
        }

        foreach (var button in _characterButtons)
        {
            if (button.IsDisabled) { continue; }

            if (IsCharacterTaken(button.Character.ID, false))
            {
                button.SetDisabled();
            }
        }

        foreach (var player in _players)
        {
            if (player.ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (player.IsLockedIn)
            {
                _lockInButton.interactable = false;
                break;
            }

            if (IsCharacterTaken(player.CharacterId, false))
            {
                _lockInButton.interactable = false;
                break;
            }

            _lockInButton.interactable = true;

            break;
        }
    }

    private bool IsCharacterTaken(int characterId, bool checkAll)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (!checkAll)
            {
                if (_players[i].ClientId == NetworkManager.Singleton.LocalClientId) { continue; }
            }

            if (_players[i].IsLockedIn && _players[i].CharacterId == characterId)
            {
                return true;
            }
        }

        return false;
    }

}