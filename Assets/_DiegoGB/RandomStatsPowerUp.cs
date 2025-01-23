using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RandomStatsPowerUp : NetworkBehaviour
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            int randomHp = Random.Range(_hpRandomMin, _hpRandomMax);
            int randomPhysicalDamage = Random.Range(_physicalDamageMin, _physicalDamageMax);
            int randomMagicalDamage = Random.Range(_magicalDamageMin, _magicalDamageMax);
            int randomPhysicalDefense = Random.Range(_physicalDefenseMin, _physicalDefenseMax);
            int randomMagicalDefense = Random.Range(_magicalDefenseMin, _magicalDefenseMax);
            float randomMovementSpeed = Random.Range(_movementSpeedMin, _movementSpeedMax);
            float randomAttackSpeed = Random.Range(_attackSpeedMin, _attackSpeedMax);
            float randomCooldownReduction = Random.Range(_cooldownReductionMin, _cooldownReductionMax);

            Debug.Log($"{other.gameObject} ha sido randomizado con las siguientes stats:\n" +
            $" Hp: {randomHp}\n" +
            $" Hp: {randomPhysicalDamage}\n" +
            $" Hp: {randomMagicalDamage}\n" +
            $" Hp: {randomPhysicalDefense}\n" +
            $" Hp: {randomMagicalDefense}\n" +
            $" Hp: {randomMovementSpeed}\n" +
            $" Hp: {randomAttackSpeed}\n" +
            $" Hp: {randomCooldownReduction}");

            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}
