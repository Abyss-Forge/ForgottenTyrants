using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Systems.BehaviourTree;

public class BossController : Entity
{

    [SerializeField] private float _targetSelectionInterval = 6f;
    [SerializeField] private float _attackInterval = 3f;
    [SerializeField] float _meleeRange = 10f;
    [SerializeField] float _decayValue = 5f;
    [SerializeField] float _decayInterval = 3f;
    //[SerializeField] List<GameObject> _players = new List<GameObject>();
    [SerializeField] float _gravityEventEffectDuration = 10f;
    [SerializeField] float _damageBoostEffectDuration = 5f;
    [SerializeField] float _disappearEffectDuration = 3f;

    [Header("Power Up settings")]
    [SerializeField] GameObject _powerUpPrefab;
    [SerializeField] float _spawnRadius = 100f;
    [SerializeField] int _spawnCount = 3;
    [SerializeField] float _heightOffset = 2f;

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

    private BehaviorSequence _rootSequence;
    private Terrain _terrain;
    private Dictionary<GameObject, float> _originalJumpForces = new Dictionary<GameObject, float>();


    Dictionary<Player, float> _playerAggroList = new Dictionary<Player, float>();
    private Player _currentTarget = null;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            // Solo el servidor (o el host, si eres host) inicializa la lógica
            // si quieres que sea autoritativa. Si NO, puedes ponerlo en Start() normal.
            //InitializeBehaviorTree();
            CurrentHp = BaseStats.Health;

            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject obj in playerObjects)
            {
                Player player = obj.GetComponent<Player>();
                if (player != null && !_playerAggroList.ContainsKey(player))
                {
                    _playerAggroList[player] = 0;
                }
                // if (!_players.Contains(obj))
                // {
                //     _players.Add(obj);
                // }
            }
            _terrain = Terrain.activeTerrain;
        }
    }

    void Start()
    {
        InitializeBehaviorTree();
        CurrentHp = BaseStats.Health;

        // GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        // foreach (GameObject obj in playerObjects)
        // {
        //     Player player = obj.GetComponent<Player>();
        //     if (player != null && !_playerAggroList.ContainsKey(player))
        //     {
        //         _playerAggroList[player] = 0;
        //     }
        // }

        // foreach (GameObject obj in playerObjects)
        // {
        //     GameObject player = obj;
        //     if (player != null)
        //     {
        //         _players.Add(player);
        //     }
        // }
    }

    void Update()
    {
        _rootSequence.Execute();

        //if (Input.GetKeyDown(KeyCode.P)) SwapPositionsRandomly();
        //if (Input.GetKeyDown(KeyCode.T)) StartCoroutine(EventLowGravity());
        if (IsClient)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TriggerDamageBoostServerRpc();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TriggerDissapearServerRpc();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                RequestPowerUpsServerRpc(); // El cliente pide al servidor que spawnee power-ups
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TriggerStormServerRpc();
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                TriggerLowGravityServerRpc();
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                TriggerSwapPositionsServerRpc();
            }

        }

    }

    void InitializeBehaviorTree()
    {
        _rootSequence = new BehaviorSequence();


        var decayAggroNode = new TimerNode(_decayInterval, new DecayAggroNode(this));
        var selectTargetNode = new TimerNode(_targetSelectionInterval, new SelectBestTargetNode(this));
        var attackTargetNode = new TimerNode(_attackInterval, new AttackTargetNode(this));

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
        if (_playerAggroList.Count == 0) return;

        float highestScore = float.MinValue;

        foreach (var entry in _playerAggroList)
        {
            Player player = entry.Key;
            float aggro = entry.Value;
            float distance = Vector3.Distance(transform.position, player.transform.position);
            float health = player.CurrentHp;

            float normalizedDamage = aggro / 100;
            float normalizedDistance = 1 - (distance / 100);
            float normalizedHealth = 1 - ((float)health / 100);

            float weightDamage = 1.0f;
            float weightDistance = 1.0f;
            float weightHealth = 1.0f;

            float score = (normalizedDamage * weightDamage + normalizedDistance * weightDistance + normalizedHealth * weightHealth) / (weightDamage + weightDistance + weightHealth);

            //Debug.Log($"Player: {player.name}, Aggro: {aggro}, Distance: {distance}, Health: {health}, Score: {score}");

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

        float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);

        if (distance <= _meleeRange)
        {
            PerformMeleeAttack();
        }
        else PerformRangedAttack();
    }

    void PerformMeleeAttack()
    {
        //Debug.Log($"Performing melee attack on {_currentTarget.name}");
    }

    void PerformRangedAttack()
    {
        //Debug.Log($"Performing ranged attack on {_currentTarget.name}");
    }

    public void TakeDamage(Player player, int damage)
    {
        CurrentHp -= damage;
        Debug.Log($"Boss recibe {damage} de daño. Vida restante: {CurrentHp}");
        AddAggro(player, damage);
        if (CurrentHp <= 0) Die();
    }

    public void AddAggro(Player player, float amount)
    {
        if (!_playerAggroList.ContainsKey(player))
        {
            _playerAggroList[player] = 0;
        }
        _playerAggroList[player] += amount;
    }

    public void ResetAggro(Player player)
    {
        if (_playerAggroList.ContainsKey(player))
        {
            _playerAggroList[player] = 0;
            Debug.Log($"Aggro reseteado para {player.name}.");
        }
    }

    public float GetAggro(Player player)
    {
        return _playerAggroList.ContainsKey(player) ? _playerAggroList[player] : 0;
    }

    public Player GetCurrentTarget()
    {
        return _currentTarget;
    }

    public void CalculateDecay()
    {
        List<Player> players = new List<Player>(_playerAggroList.Keys);

        foreach (Player player in players)
        {
            _playerAggroList[player] -= _decayValue;
            if (_playerAggroList[player] < 0) _playerAggroList[player] = 0;
        }
    }
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }

    #region 1- DAMAGE BOOST EVENT

    [ServerRpc(RequireOwnership = false)]
    public void TriggerDamageBoostServerRpc()
    {
        Debug.Log("Evento de boosteo de damage activado por un cliente.");
        TriggerDamageBoostClientRpc();
    }

    [ClientRpc]
    private void TriggerDamageBoostClientRpc()
    {
        StartCoroutine(EventDamageBoost());
    }

    IEnumerator EventDamageBoost()
    {
        _modifiedStats.ChangePhysicalDamage(_baseStats.PhysicalDamage * 2);
        _modifiedStats.ChangePhysicalDamage(_baseStats.MagicalDamage * 2);


        yield return new WaitForSeconds(_damageBoostEffectDuration);

        _modifiedStats.ChangePhysicalDamage(_baseStats.PhysicalDamage);
        _modifiedStats.ChangePhysicalDamage(_baseStats.MagicalDamage);
    }

    #endregion

    #region 2- DISAPPEAR EVENT

    [ServerRpc(RequireOwnership = false)]

    public void TriggerDissapearServerRpc()
    {
        Debug.Log("Evento de invisibilidad activado por un cliente.");
        TriggerDisappearClientRpc();
    }

    [ClientRpc]
    private void TriggerDisappearClientRpc()
    {
        StartCoroutine(EventDissapear());
    }


    IEnumerator EventDissapear()
    {
        yield return StartCoroutine(MoveTo(_hidePosition.position, _moveDuration));

        yield return new WaitForSeconds(_disappearEffectDuration);

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

            // Si usas una curva de movimiento, aplica su valor
            if (_movementCurve != null)
            {
                t = _movementCurve.Evaluate(t);
            }

            // Interpolar entre la posición inicial y la final
            transform.position = Vector3.Lerp(startPosition, destination, t);

            yield return null;
        }

        // Asegúrate de llegar exactamente al destino
        transform.position = destination;
    }

    #endregion

    #region 3- POWER UPS EVENT

    [ServerRpc(RequireOwnership = false)]
    public void RequestPowerUpsServerRpc()
    {
        Debug.Log("Solicitud de power-ups recibida desde un cliente.");
        EventPowerUps();
    }

    private void EventPowerUps()
    {
        if (!IsServer) return;

        for (int i = 0; i < _spawnCount; i++)
        {
            // Generar una posición aleatoria dentro del radio
            Vector3 randomPosition = GetRandomPositionAroundBoss();

            // Instanciar el potenciador en la posición calculada
            GameObject powerUp = Instantiate(_powerUpPrefab, randomPosition, Quaternion.identity);

            NetworkObject networkObject = powerUp.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                // Spawnear el objeto en la red
                networkObject.Spawn();
            }
            else
            {
                Debug.LogError("El prefab del power-up no tiene un componente NetworkObject.");
            }
        }
    }

    #endregion

    #region 4- CLIMATE CHANGES (STORM) EVENT

    [ServerRpc(RequireOwnership = false)]
    public void TriggerStormServerRpc()
    {
        Debug.Log("Evento de tormenta activado por un cliente.");
        StartCoroutine(EventStorm());
    }

    [ClientRpc]
    private void SpawnLightningAtPositionClientRpc(Vector3 position)
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

            // Llamar a un método para generar el rayo (puedes ajustar la lógica)
            SpawnLightningAtPosition(spawnPosition);

            // Esperar el tiempo entre rayos
            yield return new WaitForSeconds(_lightningInterval);

            elapsedTime += _lightningInterval;
        }
    }

    private void SpawnLightningAtPosition(Vector3 position)
    {
        var lightning = Instantiate(_lightningPrefab, position, Quaternion.identity);
        var netObj = lightning.GetComponent<NetworkObject>();
        netObj.Spawn(); // El servidor lo spawnea y lo controla
    }

    #endregion

    #region 5- LOW GRAVITY EVENT

    [ServerRpc(RequireOwnership = false)]
    public void TriggerLowGravityServerRpc()
    {
        Debug.Log("Evento de baja gravedad activado por un cliente.");
        Vector3 lowGravity = new Vector3(0, -2f, 0);

        SetGravityClientRpc(lowGravity);

        foreach (GameObject player in GetAllPlayers())
        {
            SetPlayerJumpForce(player, 5f); // Multiplica JumpForce por 5
        }

        StartCoroutine(RestoreGravityAfterDuration());
    }


    [ClientRpc]
    private void SetGravityClientRpc(Vector3 newGravity)
    {
        Physics.gravity = newGravity;
    }

    [ClientRpc]
    private void SetPlayerJumpForceClientRpc(float newJumpForce, ClientRpcParams clientRpcParams = default)
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
        SetGravityClientRpc(defaultGravity); // Restaura la gravedad para todos los clientes

        foreach (GameObject player in GetAllPlayers())
        {
            RestorePlayerJumpForce(player); // Restaura el JumpForce original
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
            //pc.SetJumpForce(newJumpForce); // Cambia localmente en el servidor

            // Notifica al cliente
            var networkObject = player.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                var clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams { TargetClientIds = new[] { networkObject.OwnerClientId } }
                };
                SetPlayerJumpForceClientRpc(newJumpForce, clientRpcParams);
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
                var clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams { TargetClientIds = new[] { networkObject.OwnerClientId } }
                };
                SetPlayerJumpForceClientRpc(originalJumpForce, clientRpcParams);
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

    [ServerRpc(RequireOwnership = false)]
    public void TriggerSwapPositionsServerRpc()
    {
        Debug.Log("Evento de intercambio de posiciones activado por un cliente.");
        TriggerSwapPositionsClientRpc();
    }

    [ClientRpc]
    private void TriggerSwapPositionsClientRpc()
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
            //Debug.Log($"{player.name} Player position after shuffle: {player.transform.position}");
        }
        Shuffle(positionRotationPairs);

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

            //Debug.Log($"{player.name} - Old Rotation: {player.transform.rotation.eulerAngles} New Rotation: {positionRotationPairs[i].rotation.eulerAngles}");
            player.transform.position = positionRotationPairs[i].position;
            player.transform.localRotation = positionRotationPairs[i].rotation;
            //Debug.Log($"{player.name} - Updated Rotation: {player.transform.rotation.eulerAngles}");

            if (characterController != null)
            {
                characterController.enabled = true;
                animator.enabled = true;
            }
        }
    }

    private Vector3 GetRandomPositionAroundBoss()
    {
        // Generar un punto aleatorio dentro de un círculo
        Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
        float x = transform.position.x + randomCircle.x;
        float z = transform.position.z + randomCircle.y;

        // Obtener la altura del terreno en la posición (x, z)
        float y = _terrain.SampleHeight(new Vector3(x, 0, z));

        // Devolver la posición con el offset de altura
        return new Vector3(x, y + _heightOffset, z);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}

namespace Systems.BehaviourTree
{
    public abstract class BehaviorNode
    {
        public enum NodeState { Running, Success, Failure }
        protected NodeState state = NodeState.Running;
        public NodeState State => state;
        public abstract NodeState Execute();
    }
    public class TimerNode : BehaviorNode
    {
        private float interval;
        private BehaviorNode childNode;
        private float elapsedTime = 0f;

        public TimerNode(float interval, BehaviorNode childNode)
        {
            this.interval = interval;
            this.childNode = childNode;
        }

        public override NodeState Execute()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= interval)
            {
                elapsedTime = 0f;
                return childNode.Execute();
            }

            return NodeState.Running;
        }
    }
    public class DecayAggroNode : BehaviorNode
    {
        private BossController boss;

        public DecayAggroNode(BossController boss)
        {
            this.boss = boss;
        }
        public override NodeState Execute()
        {
            boss.CalculateDecay();
            return NodeState.Success;
        }
    }

    public class SelectBestTargetNode : BehaviorNode
    {
        private BossController boss;

        public SelectBestTargetNode(BossController boss)
        {
            this.boss = boss;
        }

        public override NodeState Execute()
        {
            boss.SelectBestTarget();
            return boss.GetCurrentTarget() != null ? NodeState.Success : NodeState.Failure;
        }
    }

    public class AttackTargetNode : BehaviorNode
    {
        private BossController boss;

        public AttackTargetNode(BossController boss)
        {
            this.boss = boss;
        }

        public override NodeState Execute()
        {
            boss.AttackTarget();
            return NodeState.Success;
        }
    }
    public class BehaviorSequence : BehaviorNode
    {
        private List<BehaviorNode> nodes = new List<BehaviorNode>();
        private int currentNodeIndex = 0;

        public void AddNode(BehaviorNode node)
        {
            nodes.Add(node);
        }

        public override NodeState Execute()
        {
            if (currentNodeIndex >= nodes.Count)
            {
                currentNodeIndex = 0;
                return NodeState.Success;
            }

            var currentNode = nodes[currentNodeIndex];
            var result = currentNode.Execute();

            if (result == NodeState.Running)
            {
                return NodeState.Running;
            }

            if (result == NodeState.Success)
            {
                currentNodeIndex++;
                return Execute();
            }

            return NodeState.Failure;
        }
    }
}