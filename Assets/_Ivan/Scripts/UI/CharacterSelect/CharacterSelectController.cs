using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Team
{
    public int ID;
    public int Size;
    public int CurrentPlayerCount;

    public Team(int id, int size, int currentPlayerCount = 0)
    {
        ID = id;
        Size = size;
        CurrentPlayerCount = currentPlayerCount;
    }
}

public class CharacterSelectController : NetworkBehaviour
{
    [SerializeField] private CharacterSelectionMenuController _buildController;

    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private Transform _charactersHolder, _cardsHolder, _characterInfoPanel, _introSpawnPoint;
    [SerializeField] private CharacterSelectButton _selectButtonPrefab;
    [SerializeField] private TeamSelectElement[] _teamBanners;
    [SerializeField] private PlayerCard _playerCardPrefab;
    [SerializeField] private TMP_Text _characterNameText, _joinCodeText;
    [SerializeField] private Button _lockInButton;

    private int _maxTeamSize;
    private GameObject _introInstance;
    private List<CharacterSelectButton> _characterButtons = new();
    private List<PlayerCard> _playerCards;
    private NetworkList<CharacterSelectState> _players;
    private List<Team> _teams = new();

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

        if (IsClient)
        {
            UpdateMaxTeamSize();

            foreach (var character in _characterDatabase.Characters)
            {
                var selectbuttonInstance = Instantiate(_selectButtonPrefab, _charactersHolder);
                selectbuttonInstance.Initialize(this, character);
                _characterButtons.Add(selectbuttonInstance);
            }

            for (int i = 0; i < _teamBanners.Length; i++)
            {
                Team team = new(i, _maxTeamSize);
                _teamBanners[i].SetTeam(this, team);
                _teams.Add(team);
            }

            _players.OnListChanged += HandlePlayersStateChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }

        if (IsClient)
        {
            UpdateMaxTeamSize();

            _players.OnListChanged -= HandlePlayersStateChanged;
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

    private void UpdateMaxTeamSize()
    {
        _maxTeamSize = (int)Math.Ceiling((double)_players.Count / _teamBanners.Length);
    }

    public void SelectTeam(Team team)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (_players[i].IsLockedIn) { return; }

            if (_players[i].TeamId == team.ID) { return; }

            if (IsTeamFull(team.ID, false)) { return; }

            Team playerTeam = _teams.FirstOrDefault(x => x.ID == _players[i].TeamId);
            if (playerTeam != null)
            {
                playerTeam.CurrentPlayerCount--;
            }
        }

        team.CurrentPlayerCount++;

        SelectTeamServerRpc(team.ID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectTeamServerRpc(int teamId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (IsTeamFull(teamId, true)) { return; }

            CharacterSelectState temp = _players[i];
            temp.TeamId = teamId;
            _players[i] = temp;
        }
    }

    public void SelectBuild(RaceTemplate race, ClassTemplate @class, ArmorTemplate armor, TrinketTemplate trinket)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (_players[i].IsLockedIn) { return; }

            if (_players[i].RaceId == race.UID && _players[i].ClassId == @class.UID &&
            _players[i].ArmorId == armor.UID && _players[i].TrinketId == trinket.UID)
            {
                return;
            }
        }

        SelectBuildServerRpc(race.UID, @class.UID, armor.UID, trinket.UID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectBuildServerRpc(ulong raceId, ulong classId, ulong armorId, ulong trinketId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!_buildController.RaceDatabase.IsValidId(raceId)) { return; }
            if (!_buildController.ClassDatabase.IsValidId(classId)) { return; }
            if (!_buildController.ArmorDatabase.IsValidId(armorId)) { return; }
            if (!_buildController.TrinketDatabase.IsValidId(trinketId)) { return; }

            CharacterSelectState temp = _players[i];
            temp.RaceId = raceId;
            temp.ClassId = classId;
            temp.ArmorId = armorId;
            temp.TrinketId = trinketId;
            _players[i] = temp;
        }
    }

    public void SelectCharacter(CharacterTemplate character)
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

        if (_introInstance != null) { Destroy(_introInstance); }
        _introInstance = Instantiate(character.IntroPrefab.gameObject, _introSpawnPoint);

        SelectCharacterServerRpc(character.ID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectCharacterServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!_characterDatabase.IsValidId(characterId)) { return; }

            if (IsCharacterTaken(characterId, true)) { return; }

            CharacterSelectState temp = _players[i];
            temp.CharacterId = characterId;
            _players[i] = temp;
        }
    }

    public void LockIn()
    {
        SelectBuild(_buildController.SelectedRace, _buildController.SelectedClass,
            _buildController.SelectedArmor, _buildController.SelectedTrinket);

        LockInServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!_characterDatabase.IsValidId(_players[i].CharacterId)) { return; }

            if (IsCharacterTaken(_players[i].CharacterId, true)) { return; }


            CharacterSelectState temp = _players[i];
            temp.IsLockedIn = true;
            _players[i] = temp;

            /*_players[i] = new CharacterSelectState(
                _players[i].ClientId,
                _players[i].TeamId,
                true,
                _players[i].CharacterId
            );*/
        }

        foreach (var player in _players)
        {
            if (!player.IsLockedIn) return;
        }

        foreach (var player in _players)
        {
            HostManager.Instance.SetCharacter(player.ClientId, _characterDatabase.GetById(player.CharacterId));

            HostManager.Instance.SetTeam(player.ClientId, player.TeamId);

            var race = _buildController.RaceDatabase.GetById(player.RaceId);
            var @class = _buildController.ClassDatabase.GetById(player.ClassId);
            var armor = _buildController.ArmorDatabase.GetById(player.ArmorId);
            var trinket = _buildController.TrinketDatabase.GetById(player.TrinketId);

            Debug.Log($"{player.RaceId}");
            Debug.Log($"{race.name}");

            HostManager.Instance.SetCharacterBuild(player.ClientId, race, @class, armor, trinket);
        }

        HostManager.Instance.StartGame();
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        //update player cards
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

        //update character select buttons
        foreach (var button in _characterButtons)
        {
            if (button.IsDisabled) { continue; }

            button.SetDisabled(IsCharacterTaken(button.Character.ID, false));
        }

        //update team select buttons
        foreach (var banner in _teamBanners)
        {
            if (banner.IsDisabled) { continue; }

            banner.SetDisabled(IsTeamFull(banner.Team.ID, false));
        }

        //update lock in button
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

            /*if (IsTeamFull(player.TeamId, false))
            {
                _lockInButton.interactable = false;
                break;
            }*/

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

    private bool IsTeamFull(int teamId, bool checkAll)
    {
        for (int i = 0, playersInTeam = 0; i < _players.Count; i++)
        {
            if (!checkAll)
            {
                if (_players[i].ClientId == NetworkManager.Singleton.LocalClientId) { continue; }
            }

            if (_players[i].IsLockedIn && _players[i].TeamId == teamId)
            {
                playersInTeam++;
            }

            if (playersInTeam >= _maxTeamSize)
            {
                return true;
            }
        }

        return false;
    }

}