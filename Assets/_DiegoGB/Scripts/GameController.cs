using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Systems.EventBus;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _starting;
    [SerializeField] private float _minAlpha = 0.3f;
    [SerializeField] private float _maxAlpha = 1f;
    [SerializeField] private float _velocity = 1f;
    [SerializeField] private string _prepareTime = "00:30";
    [SerializeField] private string _gameTime = "15:00";

    [SerializeField] private Transform _alliesContainer; // Contenedor para aliados
    [SerializeField] private Transform _enemiesContainer; // Contenedor para enemigos
    [SerializeField] private GameObject _characterFramePrefab; // Prefab del cuadro

    //private List<Player> _allies = new List<Player>();
    //private List<Player> _enemies = new List<Player>();
    private NetworkList<SyncedPlayerData> _syncedPlayers;
    private NetworkVariable<float> currentTime = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private NetworkVariable<bool> countingDown = new NetworkVariable<bool>(
       false,
       NetworkVariableReadPermission.Everyone,
       NetworkVariableWritePermission.Server
   );

    private NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    void Awake()
    {
        _syncedPlayers = new();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentTime.Value = TimeStringToSeconds(_prepareTime);
            countingDown.Value = true;
            gameStarted.Value = false;

            // Poblar la lista de jugadores al inicio
            foreach (var kvp in HostManager.Instance.ClientDataDict)
            {
                var clientId = kvp.Key;
                var clientData = kvp.Value;

                SyncedPlayerData playerData = new SyncedPlayerData
                {
                    ClientId = clientId,
                    TeamId = clientData.TeamId,
                };
                _syncedPlayers.Add(playerData);
            }
            BlockAnyMovementClientRpc();
            PopulateContainerClientRpc();
        }
        if (IsClient)
        {
            _syncedPlayers.OnListChanged += OnSyncedPlayersChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _syncedPlayers.OnListChanged -= OnSyncedPlayersChanged;
        }
    }

    private void OnSyncedPlayersChanged(NetworkListEvent<SyncedPlayerData> changeEvent)
    {
        PopulateContainer();
    }

    void Update()
    {
        TimerClock();
        GameStartingAnimation();
    }

    private void AddPlayerToSyncedList(ulong clientId, ClientData clientData)
    {
        SyncedPlayerData playerData = new SyncedPlayerData
        {
            ClientId = clientId,
            TeamId = clientData.TeamId

        };
        _syncedPlayers.Add(playerData);
    }

    IEnumerator BlockAnyMovement()
    {
        yield return new WaitForSeconds(.5f);
        EventBus<PlayerMovementEvent>.Raise(new PlayerMovementEvent { Activate = false });
    }

    [Rpc(SendTo.ClientsAndHost)]
    void BlockAnyMovementClientRpc()
    {
        StartCoroutine(BlockAnyMovement());
    }

    [Rpc(SendTo.ClientsAndHost)]
    void PopulateContainerClientRpc()
    {
        StartCoroutine(PopulateContainer());
    }

    IEnumerator PopulateContainer()
    {
        yield return new WaitForSeconds(.5f);

        //List<Player> players = FindObjectsByType<Player>(FindObjectsSortMode.None).ToList();

        // Itera sobre la lista sincronizada
        foreach (var playerData in _syncedPlayers)
        {
            Transform container = playerData.TeamId == 0 ? _alliesContainer : _enemiesContainer;

            // Instancia un nuevo cuadro
            GameObject frame = Instantiate(_characterFramePrefab, container);

            // Asignar los datos al cuadro
            //frame.transform.Find("CharacterImage").GetComponent<Image>().sprite = GetCharacterSprite(clientData.CharacterId);
            //frame.transform.Find("HealthSlider").GetComponent<Slider>().value = clientData.Health;
            //frame.transform.Find("KDA").GetComponent<Text>().text = $"ID: {clientId}";
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void UpdatePlayerHealthServerRpc(ulong clientId, float newHealth)
    {
        for (int i = 0; i < _syncedPlayers.Count; i++)
        {
            if (_syncedPlayers[i].ClientId == clientId)
            {
                var playerData = _syncedPlayers[i];
                //playerData.Health = Mathf.Clamp01(newHealth);
                _syncedPlayers[i] = playerData;
                break;
            }
        }
    }

    /*foreach (var player in players)
    {
        // Determina el contenedor según el equipo del jugador
        Transform cont = player._playerData.TeamId == 0 ? _alliesContainer : _enemiesContainer;

        // Instanciar el cuadro
        GameObject frame = Instantiate(_characterFramePrefab, cont);

        //frame.transform.Find("CharacterImage").GetComponent<Image>().sprite = GetCharacterSprite(client.CharacterId);
        frame.transform.Find("Player health").GetComponent<Slider>().value = player.CurrentHp;
        //frame.transform.Find("KDA").GetComponent<Text>().text = $"ID: {client.ClientId}";
    }*/


    /*public void UpdateHealth(Player character, float newHealth)
    {
        character.BaseStats.Hp = Mathf.Clamp01(newHealth); // Asegura que la vida esté entre 0 y 1
        // Opcional: Actualiza dinámicamente el UI (puedes optimizar esto según el sistema de eventos del juego)
        PopulateContainer(alliesContainer, allies);
        PopulateContainer(enemiesContainer, enemies);
    }*/

    IEnumerator SmoothHealthChange(Slider slider, float targetValue)
    {
        float currentValue = slider.value;
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(currentValue, targetValue, elapsedTime / duration);
            yield return null;
        }

        slider.value = targetValue;
    }

    private void TimerClock()
    {
        // Solo el Servidor actualiza la cuenta regresiva
        if (IsServer && countingDown.Value)
        {
            currentTime.Value -= Time.deltaTime;

            if (currentTime.Value <= 0f && !gameStarted.Value)
            {
                StartGameServerRpc();

            }
            else if (currentTime.Value <= 0f && gameStarted.Value)
            {
                FinishGame();
            }
        }
        _timer.text = SecondsToTimeString(currentTime.Value);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        gameStarted.Value = true;
        HideStartingTextClientRpc();
        ActivateMovementPlayersClientRpc();
        currentTime.Value = TimeStringToSeconds(_gameTime);
        countingDown.Value = true;
        //PlayersToSpawn();
    }

    /*void PlayersToSpawn()
    {
        // Reposiciona a los players
        CharacterSpawner spawner = FindObjectOfType<CharacterSpawner>();
        if (spawner != null)
        {
            spawner.ResetPlayersPositions();
        }
    }*/

    private void FinishGame()
    {
        currentTime.Value = 0f;
        countingDown.Value = false;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HideStartingTextClientRpc()
    {
        StartCoroutine(HideStartingText());
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ActivateMovementPlayersClientRpc()
    {
        EventBus<PlayerMovementEvent>.Raise(new PlayerMovementEvent { Activate = true });
    }

    IEnumerator HideStartingText()
    {
        yield return new WaitForSeconds(.5f);
        Color _actualColor = _starting.color;
        _actualColor.a = 0;
        _starting.color = _actualColor;
    }

    void GameStartingAnimation()
    {
        if (gameStarted.Value) return;
        // Mathf.PingPong genera un valor que oscila entre 0 y (alfaMaximo - alfaMinimo)
        // Al sumarle alfaMinimo, el valor oscila entre alfaMinimo y alfaMaximo.
        float newAlpha = Mathf.PingPong(Time.time * _velocity, _maxAlpha - _minAlpha) + _minAlpha;

        // Actualizamos el color del texto, conservando los valores RGB y modificando el alfa.
        Color _actualColor = _starting.color;
        _actualColor.a = newAlpha;
        _starting.color = _actualColor;

    }

    private float TimeStringToSeconds(string timeString)
    {
        string[] split = timeString.Split(':');
        if (split.Length != 2)
            return 0;

        int minutes = int.Parse(split[0]);
        int seconds = int.Parse(split[1]);
        return minutes * 60 + seconds;
    }

    // Convierte un valor en segundos a un string en formato "MM:SS"
    private string SecondsToTimeString(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

// Clase para manejar datos de personajes
[System.Serializable]
public class CharacterData
{
    public Sprite characterImage;
    public int kills;
    public int deaths;
    public int assists;
}


[System.Serializable]
public struct SyncedPlayerData : INetworkSerializable, IEquatable<SyncedPlayerData>
{
    public ulong ClientId;
    public int TeamId; // Team (0: Allies, 1: Enemies)
    //public float Health; // Player's health (0 to 1)
    //public int CharacterId; // ID for the character's sprite

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref TeamId);
        //serializer.SerializeValue(ref Health);
        ///serializer.SerializeValue(ref CharacterId);
    }

    // Implementing IEquatable<T>
    public bool Equals(SyncedPlayerData other)
    {
        return ClientId == other.ClientId &&
               TeamId == other.TeamId;
        //Health == other.Health &&
        //CharacterId == other.CharacterId;
    }

    // Override GetHashCode and Equals to ensure proper equality checks
    public override bool Equals(object obj)
    {
        return obj is SyncedPlayerData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ClientId, TeamId);
    }
}
