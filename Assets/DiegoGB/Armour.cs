using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armour
{
    [SerializeField] string _name;
    [SerializeField] Stats _stats;

    public string Name => _name;
    public Stats Stats => _stats;

    public Armour(string name, Stats stats)
    {
        _name = name;
        _stats = stats;
    }
    public Armour(ArmourTemplate selectedArmour)
    {
        _name = selectedArmour.Name;
        _stats = selectedArmour.Stats;
    }

}
