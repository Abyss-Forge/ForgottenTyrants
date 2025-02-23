using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Utils.Extensions;

[RequireComponent(typeof(DamageableBehaviour), typeof(BuffableBehaviour))]
public class BossDamager : MonoBehaviour
{
    public DamageableBehaviour _damageable;
    BuffableBehaviour _buffable;

    [SerializeField] private BossController _bossController;
    [SerializeField] private GameController _gameController;
    [SerializeField] private TMP_Text _team1PointsText, _team2PointsText;

    private bool _isInvincible;
    private List<int> _alreadyAppliedDataHashes = new();    //TODO: dynamyc empty

    void Awake()
    {
        _damageable = GetComponent<DamageableBehaviour>();
        _buffable = GetComponent<BuffableBehaviour>();
        _damageable.Initialize((int)_bossController.BaseStats.Health);
        _buffable.Initialize(_bossController.BaseStats);
    }

    void OnEnable()
    {
        _damageable.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        _damageable.OnDeath -= HandleDeath;
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
                _damageable.Damage((int)damage);
                UpdateScorePoints(damageData.AbilityData.TeamId, (int)damage);
                Debug.Log("Damaging");
            }
            else if (data is HealData healData)
            {
                Debug.Log("Healing");
                _damageable.Heal((int)healData.HealAmount);
            }
            else if (data is BuffData buffData)
            {
                Debug.Log("Buffing");
                _buffable.ApplyBuffFromData(buffData);
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