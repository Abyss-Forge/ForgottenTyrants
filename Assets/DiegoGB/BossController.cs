using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BossController : Entity
{
    [SerializeField] private float _targetSelectionInterval = 6f;
    [SerializeField] private float _attackInterval = 3f;
    [SerializeField] float _meleeRange = 10f;
    [SerializeField] float _aggroMultiplier = 2f;
    [SerializeField] float _decayValue = 5f;
    [SerializeField] float _decayInterval = 3f;
    [SerializeField] Dictionary<Player, float> _playerAggroList = new Dictionary<Player, float>();
    private Player _currentTarget = null;
    private float _decayTimer = 0f;
    private float _targetSelectionTimer = 0f;
    private float _attackTimer = 0f;



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
        //Inicializar _playerAggroList con la lista de jugadores online
    }

    // Update is called once per frame
    protected override void Update()
    {
        _decayTimer += Time.deltaTime;
        if (_decayTimer >= _decayInterval)
        {
            CalculateDecay();
            _decayTimer = 0f;
        }

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

    void CalculateDecay()
    {
        List<Player> players = new List<Player>(_playerAggroList.Keys);

        foreach (Player player in players)
        {
            _playerAggroList[player] -= _decayValue;
            if (_playerAggroList[player] < 0) _playerAggroList[player] = 0;
            Debug.Log($"{player.name} pierde aggro por decaimiento. Aggro actual: {_playerAggroList[player]}");
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
}
