using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    [SerializeField] string _name;
    [SerializeField] Stats _stats;

    public string Name => _name;
    public Stats Stats => _stats;

    public Weapon(string name, Stats stats)
    {
        _name = name;
        _stats = stats;
    }
    public Weapon(WeaponTemplate selectedWeapon)
    {
        _name = selectedWeapon.Name;
        _stats = selectedWeapon.Stats;
    }
}
