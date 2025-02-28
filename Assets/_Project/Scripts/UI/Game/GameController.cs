using System;
using System.Collections;
using System.Threading.Tasks;
using Systems.EventBus;
using Systems.ServiceLocator;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    [SerializeField] private BossDamager _bossDamager;

    [Header("UI - Timers & messages")]
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _starting;
    [SerializeField] private string _prepareTime = "00:30";
    [SerializeField] private string _gameTime = "15:00";

    [Header("UI - Team scores")]
    [SerializeField] private TMP_Text _team1PointsText;
    [SerializeField] private TMP_Text _team2PointsText;

    [Header("UI - Health bars")]
    [SerializeField] private Slider _playerHealthSlider;
    [SerializeField] private Slider _bossHealthSlider;

    [Header("UI - Net stats")]
    [SerializeField] private TextMeshProUGUI _fpsStats;
    [SerializeField] private TextMeshProUGUI _pingStats;
    private float _updateInterval = 1f;

    [Header("Text animation values")]
    [SerializeField] private float _minAlpha = 0.3f;
    [SerializeField] private float _maxAlpha = 1f;
    [SerializeField] private float _velocity = 1f;

    [Header("Character frame containers")]
    [SerializeField] private Transform _alliesContainer;
    [SerializeField] private Transform _enemiesContainer;
    [SerializeField] private GameObject _characterFramePrefab;

    private float _deltaTime = 0.0f;
    private int _team1Points;
    private int _team2Points;
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

    EventBinding<PlayerRespawnEvent> _playerRespawnEventBinding;

    void Awake()
    {
        _syncedPlayers = new();
    }

    void Start()
    {
        // Actualiza las estadísticas de FPS y Ping cada "updateInterval" segundos
        InvokeRepeating(nameof(ShowFps), 1f, _updateInterval);
        InvokeRepeating(nameof(ShowPing), 1f, _updateInterval);
    }

    public override async void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Configura el temporizador de preparación y activa la cuenta regresiva
            currentTime.Value = TimeStringToSeconds(_prepareTime);
            countingDown.Value = true;
            gameStarted.Value = false;

            CreateSyncList();

            // Bloquea el movimiento y actualiza los contenedores de personajes
            BlockAnyMovementClientRpc();
            PopulateContainerClientRpc();

            _bossDamager.OnDamage += (_) => ShowBossHealth();

            await Task.Delay(100); //saluditos
            _bossHealthSlider.maxValue = _bossDamager.Health;
            _bossHealthSlider.value = _bossDamager.Health;
        }
        if (IsClient)
        {
            // Suscribe cambios en la lista sincronizada para actualizar la UI
            _syncedPlayers.OnListChanged += OnSyncedPlayersChanged;

            // Registra el evento de reaparición
            _playerRespawnEventBinding = new EventBinding<PlayerRespawnEvent>(HandleRespawn);
            EventBus<PlayerRespawnEvent>.Register(_playerRespawnEventBinding);

            // Actualiza la salud del jugador local y suscribe eventos de daño para refrescar la UI
            StartCoroutine(UpdateCurrentHp());

            await Task.Delay(100); //saluditos
            ServiceLocator.Global.Get(out DamageableBehaviour damageable);
            damageable.OnDamage += (_) => StartCoroutine(UpdateCurrentHp());
            damageable.OnDamage += (_) => ShowPlayerHealth();

            ServiceLocator.Global.Get(out DamageableBehaviour damage);
            _playerHealthSlider.maxValue = damage.Health;
            _playerHealthSlider.value = damage.Health;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            // Desuscribe los eventos y elimina el binding del EventBus
            _syncedPlayers.OnListChanged -= OnSyncedPlayersChanged;
            ServiceLocator.Global.Get(out DamageableBehaviour damageable);
            damageable.OnDamage -= (_) => StartCoroutine(UpdateCurrentHp());
            damageable.OnDamage -= (_) => ShowPlayerHealth();
            EventBus<PlayerRespawnEvent>.Deregister(_playerRespawnEventBinding);
        }
        if (IsServer)
        {
            _bossDamager.OnDamage += (_) => ShowBossHealth();
        }
    }

    void Update()
    {
        TimerClock();
        GameStartingAnimation();

        // Suaviza el deltaTime para el cálculo de FPS
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
    }

    // Maneja la reaparición del jugador actualizando la salud local y la UI
    private void HandleRespawn()
    {
        StartCoroutine(UpdateCurrentHp());
        ShowPlayerHealth();
    }

    IEnumerator UpdateCurrentHp()
    {
        yield return new WaitForSeconds(.5f);
        // Obtiene el DamageableBehaviour del jugador local usando el ServiceLocator (esto es local)
        if (ServiceLocator.Global.TryGet<DamageableBehaviour>(out DamageableBehaviour damage))
        {
            Debug.Log($"[Client {NetworkManager.Singleton.LocalClientId}] Health local value: {damage.Health}");
            UpdateMyHealthServerRpc(damage.Health);
        }
        else
        {
            Debug.LogWarning("DamageableBehaviour not found in ServiceLocator");
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

        // Aquí el servidor actualizará la salud del jugador en la NetworkList.
        // Usamos el LocalClientId (o el id del jugador que envía) para identificar a quién actualizar.
        for (int i = 0; i < _syncedPlayers.Count; i++)
        {
            if (_syncedPlayers[i].ClientId == senderClientId)
            {
                SyncedPlayerData data = _syncedPlayers[i];
                data.Health = newHealth;
                _syncedPlayers[i] = data;
                Debug.Log($"health updated {senderClientId} to {newHealth}");
                break;
            }
        }
        PopulateContainerClientRpc();
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

    // Se llama cuando hay cambios en la lista sincronizada y actualiza la UI
    private void OnSyncedPlayersChanged(NetworkListEvent<SyncedPlayerData> changeEvent)
    {
        Debug.Log($"[Client] OnSyncedPlayersChanged: Type = {changeEvent.Type} for ClientId {changeEvent.Value.ClientId} with Health = {changeEvent.Value.Health}");
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
                float ping = transport.GetCurrentRtt(0);
                _pingStats.text = $"Ping: {Mathf.Round(ping)} ms";
            }
            else
            {
                _pingStats.text = "Ping: N/A";
            }
        }
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

    IEnumerator BlockAnyMovement()
    {
        yield return new WaitForSeconds(.5f);
        EventBus<PlayerMovementEvent>.Raise(new PlayerMovementEvent { Activate = false });
    }

    IEnumerator PopulateContainer()
    {
        yield return new WaitForSeconds(.5f);

        // Limpia los contenedores de aliados y enemigos
        foreach (Transform child in _alliesContainer)
            Destroy(child.gameObject);
        foreach (Transform child in _enemiesContainer)
            Destroy(child.gameObject);

        // Itera sobre la lista sincronizada
        foreach (var playerData in _syncedPlayers)
        {
            Transform container = playerData.TeamId == 0 ? _alliesContainer : _enemiesContainer;

            // Instancia un nuevo cuadro
            GameObject frame = Instantiate(_characterFramePrefab, container);

            Slider healthSlider = frame.transform.Find("Player health")?.GetComponent<Slider>();
            healthSlider.value = playerData.Health;
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
                _syncedPlayers[i] = playerData;
                break;
            }
        }
    }

    void ShowPlayerHealth()
    {
        ServiceLocator.Global.Get(out DamageableBehaviour damage);
        _playerHealthSlider.value = damage.Health;
    }

    void ShowBossHealth()
    {
        UpdateBossHealthBar_ClientRPC(_bossDamager.Health);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void UpdateBossHealthBar_ClientRPC(float health)
    {
        _bossHealthSlider.value = health;
    }

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

    // Actualiza el temporizador y gestiona el inicio/fin de partida (solo en el servidor)
    private void TimerClock()
    {
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
    }

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

    // Convierte un string "MM:SS" a segundos
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

// Estructura para sincronizar datos básicos del jugador en red
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

    public bool Equals(SyncedPlayerData other)
    {
        return ClientId == other.ClientId &&
               TeamId == other.TeamId &&
         Mathf.Approximately(Health, other.Health);
        //CharacterId == other.CharacterId;
    }

    public override bool Equals(object obj)
    {
        return obj is SyncedPlayerData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ClientId, TeamId, Health);
    }
}
