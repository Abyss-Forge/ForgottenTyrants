using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Utils.Extensions;

public class BossDamager : MonoBehaviour, IDamageable, IBuffable
{
    [SerializeField] private BossController _bossController;
    [SerializeField] private GameController _gameController;
    [SerializeField] private TMP_Text _team1PointsText, _team2PointsText;

    private bool _isInvincible;
    private List<int> _alreadyAppliedDataHashes = new();    //TODO: dynamyc empty

    [Tooltip("Leave at 0 to initialize from code")]
    [SerializeField] private int _health;
    public int Health => _health;

    public event Action OnDeath;
    public event Action<int> OnDamage, OnHeal;

    public void InitializeDamageable(int health)
    {
        _health = health;
    }

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        OnDamage?.Invoke(damageAmount);

        if (_health <= 0) OnDeath?.Invoke();
    }

    public void Heal(int healAmount)
    {
        _health += healAmount;
        OnHeal?.Invoke(healAmount);
    }

    [Tooltip("Leave at 0 to initialize from code")]
    [SerializeField] private Stats _baseStats = new();
    private Stats _modifiedStats = new();
    public Stats CurrentStats => _modifiedStats;

    public event Action<float, EStat> OnBuff, OnDebuff;

    private CancellationTokenSource _tokenSource = new();

    public void InitializeBuffable(Stats defaultStats)
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
        _tokenSource = new();

        _baseStats = new Stats(defaultStats);
        _modifiedStats = new Stats(defaultStats);
    }

    public void ApplyBuffFromData(BuffData info) => ApplyBuff(info.Stat, info.Value, info.Duration, info.IsPercentual, info.IsDebuff);

    public void ApplyBuff(EStat stat, float value, float duration = -1, bool isPercentual = true, bool isDebuff = false)
    {
        float baseValue = _baseStats.Get(stat);
        float bakedValue = _modifiedStats.Get(stat);

        if (isPercentual) value *= baseValue / 100;

        if (isDebuff) value *= -1;

        bakedValue += value;
        _modifiedStats.Set(stat, bakedValue);

        if (duration > 0) Task.Run(() => ResetBuffTask(_tokenSource.Token, stat, value, duration), _tokenSource.Token);

        if (isDebuff) OnDebuff?.Invoke(duration, stat);
        else OnBuff?.Invoke(duration, stat);

        Debug.Log("Buff apply");
    }

    private async Task ResetBuffTask(CancellationToken token, EStat stat, float value, float duration)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(duration), token);

            float currentValue = _modifiedStats.Get(stat);
            currentValue -= value;
            _modifiedStats.Set(stat, currentValue);

            Debug.Log("Buff reset");
        }
        catch (TaskCanceledException) { Debug.Log("Buff reset cancelled"); }
    }

    void Awake()
    {
        InitializeBuffable(_baseStats);
        InitializeDamageable((int)_baseStats.Health);
    }

    void OnEnable()
    {
        OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        OnDeath -= HandleDeath;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_isInvincible) return;
        if (!other.gameObject.TryGetComponentInParent<NetworkObject>(out NetworkObject networkObject)) return;
        if (!networkObject.TryGetComponent<AbilityDataContainer>(out AbilityDataContainer container)) return;

        Debug.Log("1");
        foreach (var data in container.DataList.Where(x => !_alreadyAppliedDataHashes.Contains(x.AbilityData.Hash)))
        {
            if (_isInvincible) return;
            _alreadyAppliedDataHashes.Add(data.AbilityData.Hash);

            if (data is DamageData damageData)
            {
                float damage = damageData.DamageAmount * container.Multiplier;
                Damage((int)damage);
                UpdateScorePoints(damageData.AbilityData.TeamId, (int)damage);
                Debug.Log("Damaging");
            }
            else if (data is HealData healData)
            {
                Debug.Log("Healing");
                Heal((int)healData.HealAmount);
            }
            else if (data is BuffData buffData)
            {
                Debug.Log("Buffing");
                ApplyBuffFromData(buffData);
            }
        }
    }

    void UpdateScorePoints(int teamId, int damage)
    {
        _gameController.UpdateTeamPoints_ClientRpc(teamId, damage);
    }

    private void HandleDeath()
    {

    }

}