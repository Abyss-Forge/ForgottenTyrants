using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Systems.EventBus;
using Systems.ServiceLocator;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(DamageableBehaviour), typeof(BuffableBehaviour))]
public class BossDamager : MonoBehaviour
{
    DamageableBehaviour _damageable;
    BuffableBehaviour _buffable;
    [SerializeField] private BossController _bossController;
    [SerializeField] private TMP_Text _team1PointsText;
    [SerializeField] private TMP_Text _team2PointsText;
    [SerializeField] GameController _gameController;

    private List<AbilityInfoTest> _alreadyApliedInfos = new();    //TODO: dynamyc empty

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

    private void HandleDeath()
    {

    }

    void UpdateScorePoints(int teamId, int damage)
    {
        _gameController.UpdateTeamPoints_ClientRpc(teamId, damage);
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hola");
        if (!other.gameObject.TryGetComponent<NetworkObject>(out NetworkObject networkObject)) return;
        Debug.Log("0");
        if (!networkObject.TryGetComponent<InfoContainer>(out InfoContainer infoContainer)) return;

        Debug.Log("1");
        foreach (var info in infoContainer.InfoList.Where(x => !_alreadyApliedInfos.Contains(x)))
        {
            //_alreadyApliedInfos.Add(info);
            Debug.Log("2");
            _damageable.Damage((int)info.DamageAmount);
            Debug.Log("Equipo: " + info.TeamId);
            UpdateScorePoints(info.TeamId, (int)info.DamageAmount);
            /*if (info is DamageInfo damageInfo)
            {
                Debug.Log("3");
                EBodySection? bodyPart = GetBodyPartHit(other.contacts);
                if (bodyPart.HasValue) ApplyDamage(bodyPart.Value, damageInfo.DamageAmount);
            }
            else if (info is HealInfo healInfo)
            {
                _damageable.Heal((int)healInfo.HealAmount);
            }
            else if (info is BuffInfo buffInfo)
            {
                _buffable.ApplyBuffFromInfo(buffInfo);
            }*/
        }
    }

}
