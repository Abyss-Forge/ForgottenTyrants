using UnityEngine;

public class MultiDamagePowerUp : PowerUp
{
    [SerializeField] private int _damageTakenAmount, _damageBuffPercent;

    protected override void CalculateData()
    {
        _container.AddInfo(new DamageData(
            playerId: default,
            teamId: default,
            affectedChannel: EDamageApplyChannel.EVERYONE,
            damageAmount: _damageTakenAmount,
            damageType: EElementalType.PHYSIC)
        );

        _container.AddInfo(new BuffData(
           playerId: default,
           teamId: default,
           affectedChannel: EDamageApplyChannel.EVERYONE,
           stat: EStat.PHYSIC_DAMAGE,
           value: _damageBuffPercent,
           isPercentual: true,
           duration: 20
           )
       );
    }

}