using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trinket
{
    [SerializeField] string _name;
    [SerializeField] Stats _stats;

    public string Name => _name;
    public Stats Stats => _stats;

    public Trinket(string name, Stats stats)
    {
        _name = name;
        _stats = stats;
    }
    public Trinket(TrinketTemplate selectedTrinket)
    {
        _name = selectedTrinket.name;
        _stats = selectedTrinket.Stats;
    }

}
