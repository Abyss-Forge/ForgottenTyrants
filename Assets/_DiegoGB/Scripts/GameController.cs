using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Systems.EventBus;
using Systems.ServiceLocator;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _starting;
    [SerializeField] private TMP_Text _team1PointsText;
    [SerializeField] private TMP_Text _team2PointsText;
    [SerializeField] private float _minAlpha = 0.3f;
    [SerializeField] private float _maxAlpha = 1f;
    [SerializeField] private float _velocity = 1f;
    [SerializeField] private string _prepareTime = "00:30";
    [SerializeField] private string _gameTime = "15:00";
    [SerializeField] private Slider _playerHealth;
    [SerializeField] private Slider _bossHealth;

    [SerializeField] private Transform _alliesContainer; // Contenedor para aliados
    [SerializeField] private Transform _enemiesContainer; // Contenedor para enemigos
    [SerializeField] private GameObject _characterFramePrefab; // Prefab del cuadro

    [SerializeField] private TextMeshProUGUI _fpsStats;
    [SerializeField] private TextMeshProUGUI _pingStats;
    private float _updateInterval = 1f; // Actualiza cada 0.5 segundos
    private float _deltaTime = 0.0f;
    private int _team1Points;
    private int _team2Points;

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
        _syncedPlayers = new NetworkList<SyncedPlayerData>();
    }

    void Start()
    {
        InvokeRepeating(nameof(ShowFps), 1f, _updateInterval);
        InvokeRepeating(nameof(ShowPing), 1f, _updateInterval);
        ShowPlayerHealth();
        ShowBossHealth();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentTime.Value = TimeStringToSeconds(_prepareTime);
            countingDown.Value = true;
            gameStarted.Value = false;

            CreateSyncList();
            // Poblar la lista de jugadores al inicio

            BlockAnyMovementClientRpc();
            PopulateContainerClientRpc();
        }
        if (IsClient)
        {
            _syncedPlayers.OnListChanged += OnSyncedPlayersChanged;

            StartCoroutine(UpdateCurrentHp());
            //UpdateClientUIHealth_ClientRpc();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _syncedPlayers.OnListChanged -= OnSyncedPlayersChanged;
        }
    }

    void Update()
    {
        TimerClock();
        GameStartingAnimation();
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
    }

    IEnumerator UpdateCurrentHp()
    {
        yield return new WaitForSeconds(.5f);
        //yield return new WaitForSeconds(1f);
        // Obtiene el DamageableBehaviour del jugador local usando el ServiceLocator (esto es local)
        if (ServiceLocator.Global.TryGet<DamageableBehaviour>(out DamageableBehaviour damage))
        {
            Debug.Log($"[Cliente {NetworkManager.Singleton.LocalClientId}] Valor de Health local: {damage.Health}");
            UpdateMyHealthServerRpc(damage.Health);
        }
        else
        {
            Debug.LogWarning("No se encontró DamageableBehaviour en el ServiceLocator.");
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdateTeamPoints_ClientRpc(int teamId, int damage)
    {
        if (teamId == 0)
        {
            _team1Points += damage;
            _team1PointsText.text = _team1Points.ToString();
        }
        else
        {
            _team2Points += damage;
            _team2PointsText.text = _team2Points.ToString();
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void UpdateMyHealthServerRpc(float newHealth, RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;

        //ServiceLocator.Global.Get(out DamageableBehaviour damage);
        // Aquí el servidor actualizará la salud del jugador en la NetworkList.
        // Usamos el LocalClientId (o el id del jugador que envía) para identificar a quién actualizar.
        for (int i = 0; i < _syncedPlayers.Count; i++)
        {
            if (_syncedPlayers[i].ClientId == senderClientId)
            {
                SyncedPlayerData data = _syncedPlayers[i];
                data.Health = newHealth;
                _syncedPlayers[i] = data;
                Debug.Log($"Actualizada la salud del cliente {senderClientId} a {newHealth}");
                break;
            }
        }

    }

    void CreateSyncList()
    {
        foreach (var kvp in HostManager.Instance.ClientDataDict)
        {
            var clientId = kvp.Key;
            var clientData = kvp.Value;

            SyncedPlayerData playerData = new SyncedPlayerData
            {
                ClientId = clientId,
                TeamId = clientData.TeamId,
                Health = 85
            };
            _syncedPlayers.Add(playerData);
        }
    }

    /* [Rpc(SendTo.Server, RequireOwnership = false)]
    void SendClientHealth_ServerRpc(ulong playerId, int health)
    {
        DataSync(playerId, health);
    }

   
    void ClientHealthUpdate()
    {
        ServiceLocator.Global.Get(out DamageableBehaviour damageable);
        ServiceLocator.Global.Get(out PlayerInfo player);
        float health = Mathf.InverseLerp(0, damageable.Health, player.Stats.Health);
        SendClientHealth_ServerRpc(player.ClientData.ClientId, health);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void UpdateClientUIHealth_ClientRpc()
    {
        ActivateClientHealthUpdate();
    }

    IEnumerator ActivateClientHealthUpdate()
    {
        yield return new WaitForSeconds(.5f);
        ClientHealthUpdate();
    }

    void DataSync(ulong playerId, int health)
    {
        for (int i = 0; i < _syncedPlayers.Count; i++)
        {
            if (_syncedPlayers[i].ClientId == playerId)
            {
                var x = _syncedPlayers[i];
                x.Health = health;
                _syncedPlayers[i] = x;
            }
        }
    } */


    private void OnSyncedPlayersChanged(NetworkListEvent<SyncedPlayerData> changeEvent)
    {
        Debug.Log($"[Cliente] OnSyncedPlayersChanged: Tipo = {changeEvent.Type} para ClientId {changeEvent.Value.ClientId} con Health = {changeEvent.Value.Health}");
        StartCoroutine(PopulateContainer());
    }

    void ShowFps()
    {
        float fps = 1.0f / _deltaTime;
        _fpsStats.text = $"FPS: {Mathf.Ceil(fps)}";
    }

    void ShowPing()
    {
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            if (transport != null)
            {
                float ping = transport.GetCurrentRtt(0); // Obtener RTT en ms (NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId)
                _pingStats.text = $"Ping: {Mathf.Round(ping)} ms";
            }
            else
            {
                _pingStats.text = "Ping: N/A";
            }
        }
    }

    /*    private void AddPlayerToSyncedList(ulong clientId, ClientData clientData)
       {
           SyncedPlayerData playerData = new SyncedPlayerData
           {
               ClientId = clientId,
               TeamId = clientData.TeamId,
           };
           _syncedPlayers.Add(playerData);
       } */

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

    IEnumerator BlockAnyMovement()
    {
        yield return new WaitForSeconds(.5f);
        EventBus<PlayerMovementEvent>.Raise(new PlayerMovementEvent { Activate = false });
    }

    IEnumerator PopulateContainer()
    {
        yield return new WaitForSeconds(.5f);

        foreach (Transform child in _alliesContainer)
            Destroy(child.gameObject);
        foreach (Transform child in _enemiesContainer)
            Destroy(child.gameObject);

        //List<Player> players = FindObjectsByType<Player>(FindObjectsSortMode.None).ToList();

        // Itera sobre la lista sincronizada
        foreach (var playerData in _syncedPlayers)
        {
            Transform container = playerData.TeamId == 0 ? _alliesContainer : _enemiesContainer;

            // Instancia un nuevo cuadro
            GameObject frame = Instantiate(_characterFramePrefab, container);

            Slider healthSlider = frame.transform.Find("Player health")?.GetComponent<Slider>();
            healthSlider.value = playerData.Health;

            // Asignar los datos al cuadro
            //frame.transform.Find("CharacterImage").GetComponent<Image>().sprite = GetCharacterSprite(clientData.CharacterId);
            //frame.transform.Find("HealthSlider").GetComponent<Slider>().value = clientData.Health;
            //frame.transform.Find("KDA").GetComponent<Text>().text = $"ID: {clientId}";
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void UpdatePlayerHealthServerRpc(ulong clientId)
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

    void ShowPlayerHealth()
    {
        ServiceLocator.Global.Get(out DamageableBehaviour damage);
        _playerHealth.value = damage.Health;

        //ServiceLocator.Global.Get(out PlayerInfo player);

        //player.ClientData.ClientId
    }

    void ShowBossHealth()
    {
        float health = FindObjectOfType<BossController>().GetComponent<BossController>().CurrentHp;
        _bossHealth.value = health;

        //ServiceLocator.Global.Get(out PlayerInfo player);

        //player.ClientData.ClientId
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
    public float Health;
    //public int CharacterId; // ID for the character's sprite

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref TeamId);
        serializer.SerializeValue(ref Health);
        ///serializer.SerializeValue(ref CharacterId);
    }

    // Implementing IEquatable<T>
    public bool Equals(SyncedPlayerData other)
    {
        return ClientId == other.ClientId &&
               TeamId == other.TeamId &&
         Mathf.Approximately(Health, other.Health);
        //CharacterId == other.CharacterId;
    }

    // Override GetHashCode and Equals to ensure proper equality checks
    public override bool Equals(object obj)
    {
        return obj is SyncedPlayerData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ClientId, TeamId, Health);
    }
}
