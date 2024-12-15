using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Systems.BehaviourTree;

public class BossController : Entity
{
    private BehaviorSequence _rootSequence;

    [SerializeField] private float _targetSelectionInterval = 6f;
    [SerializeField] private float _attackInterval = 3f;
    [SerializeField] float _meleeRange = 10f;
    [SerializeField] float _aggroMultiplier = 2f;
    [SerializeField] float _decayValue = 5f;
    [SerializeField] float _decayInterval = 3f;
    [SerializeField] List<GameObject> _players = null;
    [SerializeField] float _gravityEventEffectDuration = 10f;
    [SerializeField] float _jumpForceLowGrav = 5f;

    Dictionary<Player, float> _playerAggroList = new Dictionary<Player, float>();
    private float _decayTimer = 0f;
    private float _targetSelectionTimer = 0f;
    private float _attackTimer = 0f;
    private Player _currentTarget = null;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        InitializeBehaviorTree();
        _currentHp = _stats.Hp;
        Debug.Log(_currentHp);
        Debug.Log(_stats.Hp);
        // foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        // {
        //     Player player = client.PlayerObject.GetComponent<Player>();
        //     if (player != null && !_playerAggroList.ContainsKey(player))
        //     {
        //         _playerAggroList[player] = 0;
        //         Debug.Log($"Jugador {player.name} añadido al diccionario de aggro.");
        //     }
        // }

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player"); // Encuentra todos los objetos con etiqueta "Player"

        foreach (GameObject obj in playerObjects)
        {
            Player player = obj.GetComponent<Player>(); // Obtiene el componente Player
            if (player != null && !_playerAggroList.ContainsKey(player))
            {
                _playerAggroList[player] = 0; // Inicializa con aggro 0
                Debug.Log($"Jugador {player.name} añadido al diccionario de aggro.");
            }
        }

        foreach (GameObject obj in playerObjects)
        {
            GameObject player = obj; // Obtiene el componente Player
            if (player != null)
            {
                _players.Add(player);
                Debug.Log($"Jugador {player.name} añadido al diccionario de aggro.");
            }
        }

        //Inicializar _playerAggroList con la lista de jugadores online
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwapPositionsRandomly();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(EventLowGravity());
        }

        _rootSequence.Execute();
    }

    void InitializeBehaviorTree()
    {
        _rootSequence = new BehaviorSequence();

        // Nodo de Decay Aggro
        var decayAggroNode = new DecayAggroNode(this);

        // Nodo de Selección de Mejor Objetivo
        var selectTargetNode = new SelectBestTargetNode(this);

        // Nodo de Ataque
        var attackTargetNode = new AttackTargetNode(this);

        // Añadir nodos a la secuencia
        _rootSequence.AddNode(decayAggroNode);
        _rootSequence.AddNode(selectTargetNode);
        _rootSequence.AddNode(attackTargetNode);
    }

    void SelectBestTarget()
    {
        if (_playerAggroList.Count == 0) return;

        float highestScore = float.MinValue;

        foreach (var entry in _playerAggroList)
        {
            Player player = entry.Key;
            float aggro = entry.Value;
            float distance = Vector3.Distance(transform.position, player.transform.position);
            int health = player.GetHealth();

            float score = (aggro * _aggroMultiplier) + (1 / distance) + ((100 - health) / 100);
            if (score > highestScore)
            {
                highestScore = score;
                _currentTarget = player;
            }
        }
    }

    void AttackTarget()
    {
        float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);

        if (distance <= _meleeRange)
        {
            PerformMeleeAttack();
        }
        else PerformRangedAttack();
    }

    void PerformMeleeAttack()
    {
        Debug.Log($"Performing melee attack on {_currentTarget.name}");
    }

    void PerformRangedAttack()
    {
        Debug.Log($"Performing ranged attack on {_currentTarget.name}");
    }

    public void TakeDamage(Player player, int damage)
    {
        _currentHp -= damage;
        Debug.Log($"Boss recibe {damage} de daño. Vida restante: {_currentHp}");

        AddAggro(player, damage);

        if (_currentHp <= 0) Die();
    }

    public void AddAggro(Player player, float amount)
    {
        if (!_playerAggroList.ContainsKey(player))
        {
            _playerAggroList[player] = 0;
        }
        _playerAggroList[player] += amount;
        Debug.Log($"{player.name} gana {amount} de aggro. Aggro total: {_playerAggroList[player]}");
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
            Debug.Log($"{player.name} pierde aggro por decaimiento. Aggro actual: {_playerAggroList[player]}");
        }
    }

    public void ExecuteDecay()
    {
        _decayTimer += Time.deltaTime;
        if (_decayTimer >= _decayInterval)
        {
            CalculateDecay();
            _decayTimer = 0f;
        }
    }
    public void ExecuteAttack()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _attackInterval)
        {
            if (_currentTarget != null)
            {
                AttackTarget();
            }
            else Debug.Log("Not current target");
            _attackTimer = 0f;
        }
    }
    public void ExecuteSelectTarget()
    {
        _targetSelectionTimer += Time.deltaTime;
        if (_targetSelectionTimer >= _targetSelectionInterval)
        {
            SelectBestTarget();
            _targetSelectionTimer = 0f;

            if (_currentTarget != null)
            {
                Debug.Log($"Nuevo objetivo seleccionado: {_currentTarget.name}");
            }
        }
    }

    IEnumerator EventLowGravity()
    {
        float defaultJumpForce = 0;

        Physics.gravity = new Vector3(0, -2f, 0); // Más fuerte en el eje Y
        foreach (GameObject player in _players)
        {
            defaultJumpForce = player.GetComponent<PlayerController>().JumpForce;

            player.GetComponent<PlayerController>().SetJumpForce(defaultJumpForce * 5);
        }
        yield return new WaitForSeconds(_gravityEventEffectDuration);

        Physics.gravity = new Vector3(0, -9.81f, 0);
        foreach (GameObject player in _players)
        {
            player.GetComponent<PlayerController>().SetJumpForce(defaultJumpForce);
        }
    }

    public void SwapPositionsRandomly()
    {

        if (_players == null || _players.Count < 2)
        {
            Debug.LogWarning("No hay suficientes jugadores para intercambiar posiciones.");
            return;
        }

        // Paso 1: Guardar las posiciones actuales
        List<Vector3> positions = new List<Vector3>();
        foreach (GameObject player in _players)
        {
            positions.Add(player.transform.position);
        }

        // Paso 2: Mezclar las posiciones aleatoriamente
        Shuffle(positions);

        // Paso 3: Asignar las posiciones mezcladas
        // Paso 3: Asignar las posiciones mezcladas
        for (int i = 0; i < _players.Count; i++)
        {
            GameObject player = _players[i];
            var characterController = player.GetComponent<CharacterController>();

            // Desactivar el CharacterController si existe
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            // Cambiar la posición del jugador
            player.transform.position = positions[i];

            // Reactivar el CharacterController si existe
            if (characterController != null)
            {
                characterController.enabled = true;
            }
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
}

namespace Systems.BehaviourTree
{
    public abstract class BehaviorNode
    {
        public abstract bool Execute();
    }

    public class DecayAggroNode : BehaviorNode
    {
        private BossController boss;

        public DecayAggroNode(BossController boss)
        {
            this.boss = boss;
        }

        public override bool Execute()
        {
            boss.ExecuteDecay();
            return true;
        }
    }

    public class SelectBestTargetNode : BehaviorNode
    {
        private BossController boss;

        public SelectBestTargetNode(BossController boss)
        {
            this.boss = boss;
        }

        public override bool Execute()
        {
            boss.ExecuteSelectTarget();
            return boss.GetCurrentTarget() != null;
        }
    }

    public class AttackTargetNode : BehaviorNode
    {
        private BossController boss;

        public AttackTargetNode(BossController boss)
        {
            this.boss = boss;
        }

        public override bool Execute()
        {
            if (boss.GetCurrentTarget() == null)
            {
                Debug.Log("No hay objetivo para atacar.");
                return false;
            }

            boss.ExecuteAttack();
            return true;
        }
    }

    public class BehaviorSequence : BehaviorNode
    {
        private List<BehaviorNode> nodes = new List<BehaviorNode>();

        public void AddNode(BehaviorNode node)
        {
            nodes.Add(node);
        }

        public override bool Execute()
        {
            foreach (var node in nodes)
            {
                if (!node.Execute())
                {
                    return false; // Si un nodo falla, la secuencia se detiene
                }
            }
            return true; // La secuencia fue exitosa
        }
    }
}