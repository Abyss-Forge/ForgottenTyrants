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
    [SerializeField] float _decayValue = 5f;
    [SerializeField] float _decayInterval = 3f;
    [SerializeField] List<GameObject> _players = null;
    [SerializeField] float _gravityEventEffectDuration = 10f;

    Dictionary<Player, float> _playerAggroList = new Dictionary<Player, float>();
    private Player _currentTarget = null;


    void Start()
    {
        InitializeBehaviorTree();
        CurrentHp = BaseStats.Hp;

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in playerObjects)
        {
            Player player = obj.GetComponent<Player>();
            if (player != null && !_playerAggroList.ContainsKey(player))
            {
                _playerAggroList[player] = 0;
            }
        }

        foreach (GameObject obj in playerObjects)
        {
            GameObject player = obj;
            if (player != null)
            {
                _players.Add(player);
            }
        }
    }

    void Update()
    {
        _rootSequence.Execute();

        if (Input.GetKeyDown(KeyCode.P)) SwapPositionsRandomly();
        if (Input.GetKeyDown(KeyCode.T)) StartCoroutine(EventLowGravity());
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

    public void SelectBestTarget()
    {
        if (_playerAggroList.Count == 0) return;

        float highestScore = float.MinValue;

        foreach (var entry in _playerAggroList)
        {
            Player player = entry.Key;
            float aggro = entry.Value;
            float distance = Vector3.Distance(transform.position, player.transform.position);
            int health = player.CurrentHp;

            float normalizedDamage = aggro / 100;
            float normalizedDistance = 1 - (distance / 100);
            float normalizedHealth = 1 - ((float)health / 100);

            float weightDamage = 1.0f;
            float weightDistance = 1.0f;
            float weightHealth = 1.0f;

            float score = (normalizedDamage * weightDamage + normalizedDistance * weightDistance + normalizedHealth * weightHealth) / (weightDamage + weightDistance + weightHealth);

            Debug.Log($"Player: {player.name}, Aggro: {aggro}, Distance: {distance}, Health: {health}, Score: {score}");

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
        Debug.Log($"Performing melee attack on {_currentTarget.name}");
    }

    void PerformRangedAttack()
    {
        Debug.Log($"Performing ranged attack on {_currentTarget.name}");
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

    IEnumerator EventLowGravity()
    {
        float defaultJumpForce = 0;

        Physics.gravity = new Vector3(0, -2f, 0);
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

        List<Vector3> positions = new List<Vector3>();
        foreach (GameObject player in _players)
        {
            positions.Add(player.transform.position);
        }
        Shuffle(positions);

        for (int i = 0; i < _players.Count; i++)
        {
            GameObject player = _players[i];
            var characterController = player.GetComponent<CharacterController>();

            if (characterController != null)
            {
                characterController.enabled = false;
            }

            player.transform.position = positions[i];

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

