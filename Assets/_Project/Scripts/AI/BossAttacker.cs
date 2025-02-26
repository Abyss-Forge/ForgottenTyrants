using System.Collections.Generic;
using UnityEngine;

public class BossAttacker : MonoBehaviour
{
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private float _launchForce = 10f;

    private List<IAbilityData> _abilityDataList = new();

    public void PerformRangedAttack(Stats currentStats, Transform target)
    {
        CalculateInfo(currentStats);

        Vector3 position = transform.position;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 scale = _projectilePrefab.transform.localScale * 3;
        Vector3 launchVelocity = direction * _launchForce;

        SpawnManager.Instance.SpawnProjectile(_projectilePrefab.gameObject, position, rotation, scale, launchVelocity, _abilityDataList);
    }

    private void CalculateInfo(Stats currentStats)
    {
        _abilityDataList.Clear();

        _abilityDataList.Add(new DamageData(
            playerId: default,
            teamId: default,
            affectedChannel: EDamageApplyChannel.EVERYONE,
            damageAmount: currentStats.PhysicalDamage,
            damageType: EElementalType.PHYSIC
        ));

        _abilityDataList.Add(new DamageData(
           playerId: default,
           teamId: default,
           affectedChannel: EDamageApplyChannel.EVERYONE,
           damageAmount: currentStats.MagicalDamage,
           damageType: EElementalType.MAGIC
        ));
    }

}