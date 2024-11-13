using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterClass
{
    [SerializeField] string _name;
    [SerializeField] Stats _stats;
    [SerializeField] Ability _firstAbility;
    [SerializeField] Ability _secondAbility;
    [SerializeField] Ability _thirdAbility;
    [SerializeField] Ability _ultimateAbility;

    public string Name => _name;
    public Stats Stats => _stats;

    public CharacterClass(string name, Stats stats)
    {
        _name = name;
        _stats = stats;
    }
    public CharacterClass(CharacterClassTemplate selectedClass)
    {
        _name = selectedClass.Name;
        _stats = selectedClass.Stats;
    }
}
