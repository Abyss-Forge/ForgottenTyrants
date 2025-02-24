using UnityEngine;

public class HealingPowerUp : PowerUp
{
    [SerializeField] private int _healingAmount;

    protected override void CalculateData()
    {
        _container.AddInfo(new HealData(
            playerId: default,
            teamId: default,
            affectedChannel: EDamageApplyChannel.EVERYONE,
            healAmount: _healingAmount)
        );
    }

}