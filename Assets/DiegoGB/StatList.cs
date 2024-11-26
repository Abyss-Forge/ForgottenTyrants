using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatList : MonoBehaviour
{
    [SerializeField] Stat _hp, _physicalDamage, _magicalDamage, _physicalDefense, _magicalDefense, _movementSpeed, _attackSpeed, _cooldownReduction;

    void Awake()
    {
        Debug.Log(_hp);
    }

}