using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForgottenTyrants;
using Systems.BehaviourTree;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;
using DecalProjector = UnityEngine.Rendering.Universal.DecalProjector;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class BossController : NetworkBehaviour
{
    [SerializeField] BossDamager _bossDamager;
    [SerializeField] BossAttacker _bossAttacker;

    [Header("Attacks & Aggro")]
    [SerializeField] private float _targetSelectionInterval = 6f;
    [SerializeField] private float _attackInterval = 3f;
    [SerializeField] float _meleeRange = 10f;
    [SerializeField] float _decayValue = 5f;
    [SerializeField] float _decayInterval = 3f;
    [SerializeField] float _rotationSpeed = 3f;
    [SerializeField] float _maxAttackAngle = 30f;

    [Header("Gravity settings")]
    [SerializeField] float _gravityEventEffectDuration = 10f;

    [Header("Damage boost settings")]
    [SerializeField] float _damageBoostEffectDuration = 5f;

    [Header("Power Up settings")]
    [SerializeField] NetworkObject[] _powerUpPrefabs; //Son NetworkObjects de tipo PowerUp pero por lo de la animacion tiene que referenciarse asi
    [SerializeField] float _spawnRadius = 100f;
    [SerializeField] int _spawnCount = 3;
    [SerializeField] float _heightOffsetPowerUp = 2f;

    [Header("Storm settings")]
    [SerializeField] GameObject _lightningPrefab;
    [SerializeField] Transform _thunderSpawner;
    [SerializeField] float _stormRadius;
    [SerializeField] float _stormDuration = 10;
    [SerializeField] float _lightningInterval = .5f;

    [Header("Disappear settings")]
    [SerializeField] Transform _hidePosition;
    [SerializeField] Transform _bossPosition;
    [SerializeField] private float _moveDuration = 5f;
    [SerializeField] private AnimationCurve _movementCurve;
    [SerializeField] float _disappearEffectDuration = 10f;
    [SerializeField] float _warningDuration = 3f;
    [SerializeField] private Image _warningImage;
    [SerializeField] private float _fadeDuration = .1f;
    [SerializeField] private int _repeatCount = 3;
    [SerializeField] private DecalProjector _decalProjector;
    private float _maxAlpha = 0.7f;

    [Header("Wind settings")]
    [SerializeField] GameObject _windPrefab;
    [SerializeField] float _windDuration = 10;
    [SerializeField] int _windCount;
    [SerializeField] float _heightOffsetWind = 0.04f;

    [Header("Blood settings")]
    [SerializeField] GameObject _bloodRain;
    [SerializeField] int _healingPerTick;
    [SerializeField] float _timeBetweenTicks;
    [SerializeField] int _ticks;

    private BehaviorSequence _rootSequence;
    private Terrain _terrain;
    private Dictionary<GameObject, float> _originalJumpForces = new();

    private Dictionary<Transform, float> _playerAggroDict = new();
    private Transform _currentTarget = null;
    public Transform CurrentTarget => _currentTarget;

    private List<string> _eventNames = new();

    void Awake()
    {
        InitializeBehaviorTree();

        // Configuración inicial del decal (efecto de advertencia)
        _decalProjector.fadeFactor = 0;
        Vector3 position = transform.position;
        position.y = _decalProjector.transform.position.y;
        _decalProjector.transform.position = position;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _terrain = Terrain.activeTerrain;

            RefreshPlayers();

            _eventNames.AddRange(new List<string> {
                nameof(TriggerDamageBoost_ServerRpc),
                nameof(TriggerDissapear_ServerRpc),
                nameof(RequestPowerUps_ServerRpc),
                nameof(TriggerStorm_ServerRpc),
                nameof(TriggerLowGravity_ServerRpc),
                nameof(TriggerSwapPositions_ServerRpc),
                nameof(TriggerWindEvent_ServerRpc),
                nameof(TriggerBloodEvent_ServerRpc),
            });

            float eventThreshold = (float)TimeSpan.FromMinutes(2).TotalSeconds;
            InvokeRepeating(nameof(TriggerRandomEvent), 10, eventThreshold);

            _bossDamager.OnDamage += HandleDamage;
            _bossDamager.OnDeath += Die;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            _bossDamager.OnDamage -= HandleDamage;
            _bossDamager.OnDeath -= Die;
        }
    }

    void Update()
    {
        // Ejecuta el árbol de comportamiento en cada frame
        _rootSequence.Execute();

        if (_currentTarget != null)
        {
            Vector3 direction = (_currentTarget.position - transform.position).normalized;
            direction.y = 0f;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            lookRotation *= Quaternion.Euler(0, 180f, 0); // usamos la rotacion inversa porque diego importó el modelo al reves, buen trabajo diego

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void RefreshPlayers()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag(Tag.Player);

        foreach (var obj in playerObjects)
        {
            AddAggro(obj.transform, 0);
        }
    }

    private void TriggerRandomEvent()
    {
        Debug.Log("Random event triggered");
        string methodToInvoke = _eventNames.ElementAt(Random.Range(0, _eventNames.Count - 1));
        Invoke(methodToInvoke, 0);
        _eventNames.Remove(methodToInvoke);
    }

    private void InitializeBehaviorTree()
    {
        _rootSequence = new();

        // Cada nodo se ejecuta en intervalos fijos: decaimiento de aggro, selección de objetivo y ataque
        TimerNode decayAggroNode = new(_decayInterval, new DecayAggroNode(this));
        TimerNode selectTargetNode = new(_targetSelectionInterval, new SelectBestTargetNode(this));
        TimerNode attackTargetNode = new(_attackInterval, new AttackTargetNode(this));

        _rootSequence.AddNode(decayAggroNode);
        _rootSequence.AddNode(selectTargetNode);
        _rootSequence.AddNode(attackTargetNode);
    }

    private List<GameObject> GetAllPlayers()
    {
        var players = new List<GameObject>();
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Value.PlayerObject != null)
            {
                players.Add(client.Value.PlayerObject.gameObject);
            }
        }
        return players;
    }

    public void SelectBestTarget()
    {
        if (_playerAggroDict.Count == 0) RefreshPlayers();

        float highestScore = float.MinValue;

        foreach (var entry in _playerAggroDict)
        {
            Transform player = entry.Key;
            float aggro = entry.Value;
            float distance = Vector3.Distance(transform.position, player.position);
            int health = 100; //TODO

            // Normalización de valores para calcular una puntuación
            float normalizedDamage = aggro / 100;
            float normalizedDistance = 1 - (distance / 100);
            float normalizedHealth = 1 - ((float)health / 100);

            // Pesos para cada factor (ajustables)
            float weightDamage = 1.0f;
            float weightDistance = 1.0f;
            float weightHealth = 1.0f;

            float score = (normalizedDamage * weightDamage + normalizedDistance * weightDistance + normalizedHealth * weightHealth) / (weightDamage + weightDistance + weightHealth);

            if (score > highestScore)
            {
                highestScore = score;
                _currentTarget = player;
            }
        }
    }

    public void AttackTarget()
    {
        if (_currentTarget == null) return;

        Vector2 bossPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 targetPos = new Vector2(_currentTarget.position.x, _currentTarget.position.z);

        Vector2 bossForward = new Vector2(-transform.forward.x, -transform.forward.z).normalized; // usamos forward.z inverso porque diego importó el modelo al reves, buen trabajo diego
        Vector2 toTarget = (targetPos - bossPos).normalized;

        float angle = Vector2.Angle(bossForward, toTarget);
        if (angle > _maxAttackAngle) return;

        float distance = Vector3.Distance(transform.position, _currentTarget.position);

        if (distance <= _meleeRange) PerformMeleeAttack();
        else PerformRangedAttack();
    }

    private void PerformMeleeAttack()
    {
        //TODO
        Debug.Log($"Performing melee attack on {_currentTarget.name}");
    }

    private void PerformRangedAttack()
    {
        Debug.Log($"Performing ranged attack on {_currentTarget.name}");
        _bossAttacker.PerformRangedAttack(_bossDamager.CurrentStats, CurrentTarget);
    }

    private void HandleDamage(int damage)
    {
        Debug.Log($"Boss recibe {damage} de daño. Vida restante: {_bossDamager.Health}");
        //TODO AddAggro(player, damage);
    }

    private void AddAggro(Transform player, float amount)
    {
        //TODO (Comprobar si funciona el agro de cada jugador y el boss targetea al que mas puntuación tiene)
        if (!_playerAggroDict.ContainsKey(player))
        {
            _playerAggroDict.Add(player, 0);
        }
        _playerAggroDict[player] += amount;
    }

    private void ResetAggro(Transform player)
    {
        if (_playerAggroDict.ContainsKey(player))
        {
            _playerAggroDict[player] = 0;
            Debug.Log($"Aggro reseteado para {player.name}.");
        }
    }

    public float GetAggro(Transform player)
    {
        return _playerAggroDict.ContainsKey(player) ? _playerAggroDict[player] : -1;
    }

    public void CalculateDecay()
    {
        List<Transform> players = new(_playerAggroDict.Keys);

        foreach (Transform player in players)
        {
            _playerAggroDict[player] -= _decayValue;
            if (_playerAggroDict[player] < 0) _playerAggroDict[player] = 0;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    #region 1- DAMAGE BOOST EVENT

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TriggerDamageBoost_ServerRpc()
    {
        Debug.Log("Damage boost activated");
        TriggerDamageBoost_ClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerDamageBoost_ClientRpc()
    {
        _bossDamager.ApplyBuff(stat: EStat.PHYSIC_DAMAGE, value: 100, isPercentual: true, duration: _damageBoostEffectDuration);
    }

    #endregion

    #region 2- DISAPPEAR EVENT

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TriggerDissapear_ServerRpc()
    {
        Debug.Log("Invisibility event activated");
        TriggerDisappear_ClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerDisappear_ClientRpc()
    {
        StartCoroutine(EventDissapear());
    }

    IEnumerator EventDissapear()
    {
        // Mueve el jefe a una posición oculta
        yield return StartCoroutine(MoveTo(_hidePosition.position, _moveDuration));

        yield return new WaitForSeconds(_disappearEffectDuration);

        // Muestra advertencia (efecto de fade con decal)
        StartCoroutine(DangerWarning());

        yield return new WaitForSeconds(_warningDuration);

        // Retorna el jefe a su posición original
        yield return StartCoroutine(MoveTo(_bossPosition.position, _moveDuration));
    }

    private IEnumerator MoveTo(Vector3 destination, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            if (_movementCurve != null)
            {
                t = _movementCurve.Evaluate(t);
            }

            transform.position = Vector3.Lerp(startPosition, destination, t);

            yield return null;
        }

        transform.position = destination;
    }

    private IEnumerator DangerWarning()
    {
        float originalFade = _decalProjector.fadeFactor;

        _decalProjector.fadeFactor = 0f;

        for (int i = 0; i < _repeatCount; i++)
        {
            // FADE IN (de 0 a _maxAlpha)
            float elapsedTime = 0f;
            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _fadeDuration);
                _decalProjector.fadeFactor = Mathf.Lerp(0f, _maxAlpha, t);
                yield return null;
            }

            // FADE OUT (de _maxAlpha a 0)
            elapsedTime = 0f;
            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _fadeDuration);
                _decalProjector.fadeFactor = Mathf.Lerp(_maxAlpha, 0f, t);
                yield return null;
            }
        }
        _decalProjector.fadeFactor = 0f;
    }

    #endregion

    #region 3- POWER UPS EVENT

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RequestPowerUps_ServerRpc()
    {
        Debug.Log("Power ups request activated");
        EventPowerUps();
    }

    private void EventPowerUps()
    {
        if (!IsServer) return;

        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 randomPosition = GetRandomPositionAroundBoss(_heightOffsetPowerUp, out _);
            int randomPowerUpIndex = Random.Range(0, _powerUpPrefabs.Length - 1);

            NetworkObject instance = Instantiate(_powerUpPrefabs[randomPowerUpIndex], randomPosition, Quaternion.identity);
            instance.Spawn();
        }
    }

    #endregion

    #region 4.1 - CLIMATE CHANGES (STORM) EVENT

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TriggerStorm_ServerRpc()
    {
        Debug.Log("Storm event activated");
        StartCoroutine(EventStorm());
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnLightningAtPosition_ClientRpc(Vector3 position)
    {
        if (_lightningPrefab != null)
        {
            Instantiate(_lightningPrefab, position, Quaternion.identity);
        }
    }

    private IEnumerator EventStorm()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _stormDuration)
        {
            // Generar una posición aleatoria dentro del radio alrededor del spawner
            Vector2 randomCircle = Random.insideUnitCircle * _stormRadius;
            Vector3 spawnPosition = new Vector3(
                _thunderSpawner.position.x + randomCircle.x,
                _thunderSpawner.position.y,
                _thunderSpawner.position.z + randomCircle.y
            );

            SpawnLightningAtPosition(spawnPosition);

            yield return new WaitForSeconds(_lightningInterval);

            elapsedTime += _lightningInterval;
        }
    }

    private void SpawnLightningAtPosition(Vector3 position)
    {
        var lightning = Instantiate(_lightningPrefab, position, Quaternion.identity);
        var netObj = lightning.GetComponent<NetworkObject>();
        netObj.Spawn();
    }

    #endregion

    #region 4.2 - CLIMATE CHANGES (WIND) EVENT

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TriggerWindEvent_ServerRpc()
    {
        StartCoroutine(EventWind());
    }

    private IEnumerator EventWind()
    {
        if (!IsServer) yield break;

        List<GameObject> winds = new List<GameObject>();

        for (int i = 0; i < _windCount; i++)
        {
            Vector3 randomPosition = GetRandomPositionAroundBoss(_heightOffsetWind, out Quaternion rotation);
            GameObject wind = Instantiate(_windPrefab, randomPosition, rotation);
            NetworkObject networkObject = wind.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn();
                winds.Add(wind);
            }
            else
            {
                Debug.LogError("Prefab doesnt have a NetworkObject component");
            }
        }
        yield return new WaitForSeconds(_windDuration);

        foreach (GameObject wind in winds)
        {
            NetworkObject networkObject = wind.GetComponent<NetworkObject>();
            networkObject.Despawn();
        }
    }


    #endregion

    #region 4.2 - CLIMATE CHANGES (BLOOD) EVENT

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TriggerBloodEvent_ServerRpc()
    {
        // 1. El Servidor inicia la curación.
        StartCoroutine(BossHeal());

        // 2. El Servidor ordena a TODOS los clientes que reproduzcan el efecto.
        TriggerBloodEvent_ClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerBloodEvent_ClientRpc()
    {
        // Cada cliente inicia su corrutina local de lluvia de sangre.
        StartCoroutine(BloodRainEffect());
    }

    private IEnumerator BloodRainEffect()
    {
        var ps = _bloodRain.GetComponent<ParticleSystem>();
        ps.Play();

        float totalDuration = _ticks * _timeBetweenTicks;
        yield return new WaitForSeconds(totalDuration);

        ps.Stop();
    }

    private IEnumerator BossHeal()
    {
        // El servidor va aplicando la curación poco a poco.
        for (int i = 0; i < _ticks; i++)
        {
            _bossDamager.Heal(_healingPerTick);
            yield return new WaitForSeconds(_timeBetweenTicks);
        }
    }

    #endregion

    #region 5- LOW GRAVITY EVENT

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TriggerLowGravity_ServerRpc()
    {
        Debug.Log("Low gravity event activated");
        Vector3 lowGravity = new Vector3(0, -2f, 0);

        SetGravity_ClientRpc(lowGravity);

        foreach (GameObject player in GetAllPlayers())
        {
            SetPlayerJumpForce(player, 5f);
        }

        StartCoroutine(RestoreGravityAfterDuration());
    }


    [Rpc(SendTo.ClientsAndHost)]
    private void SetGravity_ClientRpc(Vector3 newGravity)
    {
        Physics.gravity = newGravity;
    }

    [Rpc(SendTo.ClientsAndHost, AllowTargetOverride = true)]
    private void SetPlayerJumpForce_ClientRpc(float newJumpForce, RpcParams rpcParams = default)
    {
        var playerController = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetJumpForce(newJumpForce);
        }
    }

    private IEnumerator RestoreGravityAfterDuration()
    {
        yield return new WaitForSeconds(_gravityEventEffectDuration);

        Vector3 defaultGravity = new Vector3(0, -9.81f, 0);
        SetGravity_ClientRpc(defaultGravity);

        foreach (GameObject player in GetAllPlayers())
        {
            RestorePlayerJumpForce(player);
        }
    }

    private void SetPlayerJumpForce(GameObject player, float multiplier)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            if (!_originalJumpForces.ContainsKey(player))
            {
                _originalJumpForces[player] = pc.JumpForce; // Guarda el valor original
            }

            float newJumpForce = _originalJumpForces[player] * multiplier;

            // Notifica al cliente
            var networkObject = player.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                var rpcParams = new RpcSendParams
                {
                    Target = RpcTarget.Single(networkObject.OwnerClientId, RpcTargetUse.Temp)
                };
                SetPlayerJumpForce_ClientRpc(newJumpForce, rpcParams);
            }
        }
    }

    private void RestorePlayerJumpForce(GameObject player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null && _originalJumpForces.ContainsKey(player))
        {
            float originalJumpForce = _originalJumpForces[player];
            //pc.SetJumpForce(originalJumpForce); // Cambia localmente en el servidor

            // Notifica al cliente
            var networkObject = player.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                var rpcParams = new RpcParams
                {
                    Send = new RpcSendParams { Target = RpcTarget.Single(networkObject.OwnerClientId, RpcTargetUse.Temp) }
                };
                SetPlayerJumpForce_ClientRpc(originalJumpForce, rpcParams);
            }
        }
    }

    // IEnumerator EventLowGravity()
    //     {
    //         Physics.gravity = new Vector3(0, -2f, 0);

    //         foreach (GameObject player in GetAllPlayers())
    //         {
    //             SetPlayerJumpForce(player, 5f); // Multiplica JumpForce por 5
    //         }

    //         yield return new WaitForSeconds(_gravityEventEffectDuration);

    //         Physics.gravity = new Vector3(0, -9.81f, 0);

    //         foreach (GameObject player in GetAllPlayers())
    //         {
    //             RestorePlayerJumpForce(player);
    //         }
    //     }

    #endregion

    #region 6- SWAP POSITIONS EVENT

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TriggerSwapPositions_ServerRpc()
    {
        Debug.Log("Evento de intercambio de posiciones activado por un cliente.");
        TriggerSwapPositions_ClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerSwapPositions_ClientRpc()
    {
        SwapPositionsRandomly();
    }

    public void SwapPositionsRandomly()
    {
        if (GetAllPlayers() == null || GetAllPlayers().Count < 2)
        {
            Debug.LogWarning("No hay suficientes jugadores para intercambiar posiciones.");
            return;
        }

        List<(Vector3 position, Quaternion rotation)> positionRotationPairs = new List<(Vector3, Quaternion)>();
        foreach (GameObject player in GetAllPlayers())
        {
            positionRotationPairs.Add((player.transform.position, player.transform.rotation));
        }

        positionRotationPairs.Shuffle();

        for (int i = 0; i < GetAllPlayers().Count; i++)
        {
            GameObject player = GetAllPlayers()[i];
            var characterController = player.GetComponent<CharacterController>();
            Animator animator = player.GetComponentInChildren<Animator>();

            if (characterController != null)
            {
                characterController.enabled = false;
                animator.enabled = false;
            }

            player.transform.position = positionRotationPairs[i].position;
            player.transform.localRotation = positionRotationPairs[i].rotation;

            if (characterController != null)
            {
                characterController.enabled = true;
                animator.enabled = true;
            }
        }
    }

    private Vector3 GetRandomPositionAroundBoss(float heightOffset, out Quaternion rotation)
    {
        // Generar un punto aleatorio dentro de un círculo
        Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
        float x = transform.position.x + randomCircle.x;
        float z = transform.position.z + randomCircle.y;

        // Obtener la altura del terreno en la posición (x, z)
        float y = _terrain.SampleHeight(new Vector3(x, 0, z));

        // Obtener la normal del terreno en el punto (x, z)
        Vector3 terrainNormal = _terrain.terrainData.GetInterpolatedNormal(
            (x - _terrain.transform.position.x) / _terrain.terrainData.size.x,
            (z - _terrain.transform.position.z) / _terrain.terrainData.size.z
        );

        // Calcular la rotación basada en la normal
        rotation = Quaternion.LookRotation(terrainNormal);

        // Devolver la posición con el offset de altura
        return new Vector3(x, y + heightOffset, z);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}