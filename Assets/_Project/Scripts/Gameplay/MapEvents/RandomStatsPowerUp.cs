using UnityEngine;

public class RandomStatsPowerUp : PowerUp
{
   [Header("Hp Min-Max")]
   [SerializeField] int _hpRandomMin;
   [SerializeField] int _hpRandomMax;

   [Header("Physical Damage Min-Max")]
   [SerializeField] int _physicalDamageMin;
   [SerializeField] int _physicalDamageMax;

   [Header("Magical Damage Min-Max")]
   [SerializeField] int _magicalDamageMin;
   [SerializeField] int _magicalDamageMax;

   [Header("Physical Defense Min-Max")]
   [SerializeField] int _physicalDefenseMin;
   [SerializeField] int _physicalDefenseMax;

   [Header("Magical Defense Min-Max")]
   [SerializeField] int _magicalDefenseMin;
   [SerializeField] int _magicalDefenseMax;

   [Header("Movement Speed Min-Max")]
   [SerializeField] float _movementSpeedMin;
   [SerializeField] float _movementSpeedMax;

   [Header("Attack Speed Min-Max")]
   [SerializeField] float _attackSpeedMin;
   [SerializeField] float _attackSpeedMax;

   [Header("Cooldown Reduction Min-Max")]
   [SerializeField] float _cooldownReductionMin;
   [SerializeField] float _cooldownReductionMax;

   [SerializeField] private float _duration;

   protected override void CalculateData()
   {
      int randomHp = Random.Range(_hpRandomMin, _hpRandomMax);
      int randomPhysicalDamage = Random.Range(_physicalDamageMin, _physicalDamageMax);
      int randomMagicalDamage = Random.Range(_magicalDamageMin, _magicalDamageMax);
      int randomPhysicalDefense = Random.Range(_physicalDefenseMin, _physicalDefenseMax);
      int randomMagicalDefense = Random.Range(_magicalDefenseMin, _magicalDefenseMax);
      float randomMovementSpeed = Random.Range(_movementSpeedMin, _movementSpeedMax);
      float randomAttackSpeed = Random.Range(_attackSpeedMin, _attackSpeedMax);
      float randomCooldownReduction = Random.Range(_cooldownReductionMin, _cooldownReductionMax);

      _container.AddInfo(new BuffData(
         playerId: default,
         teamId: default,
         affectedChannel: EDamageApplyChannel.EVERYONE,
         stat: EStat.PHYSIC_DAMAGE,
         value: randomHp,
         isPercentual: true,
         duration: _duration)
      );

      _container.AddInfo(new BuffData(
         playerId: default,
         teamId: default,
         affectedChannel: EDamageApplyChannel.EVERYONE,
         stat: EStat.PHYSIC_DAMAGE,
         value: randomPhysicalDamage,
         isPercentual: true,
         duration: _duration)
      );

      _container.AddInfo(new BuffData(
         playerId: default,
         teamId: default,
         affectedChannel: EDamageApplyChannel.EVERYONE,
         stat: EStat.MAGIC_DAMAGE,
         value: randomMagicalDamage,
         isPercentual: true,
         duration: _duration)
      );

      _container.AddInfo(new BuffData(
         playerId: default,
         teamId: default,
         affectedChannel: EDamageApplyChannel.EVERYONE,
         stat: EStat.PHYSIC_DEFENSE,
         value: randomPhysicalDefense,
         isPercentual: true,
         duration: _duration)
      );

      _container.AddInfo(new BuffData(
         playerId: default,
         teamId: default,
         affectedChannel: EDamageApplyChannel.EVERYONE,
         stat: EStat.MAGIC_DEFENSE,
         value: randomMagicalDefense,
         isPercentual: true,
         duration: _duration)
      );

      _container.AddInfo(new BuffData(
         playerId: default,
         teamId: default,
         affectedChannel: EDamageApplyChannel.EVERYONE,
         stat: EStat.MOVEMENT_SPEED,
         value: randomMovementSpeed,
         isPercentual: true,
         duration: _duration)
      );

      _container.AddInfo(new BuffData(
         playerId: default,
         teamId: default,
         affectedChannel: EDamageApplyChannel.EVERYONE,
         stat: EStat.ATTACK_SPEED,
         value: randomAttackSpeed,
         isPercentual: true,
         duration: _duration)
      );

      _container.AddInfo(new BuffData(
         playerId: default,
         teamId: default,
         affectedChannel: EDamageApplyChannel.EVERYONE,
         stat: EStat.COOLDOWN_REDUCTION,
         value: randomCooldownReduction,
         isPercentual: true,
         duration: _duration)
      );
   }

}