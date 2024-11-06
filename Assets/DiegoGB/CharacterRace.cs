using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRace
{
    [SerializeField] string _name;
    [SerializeField] Stats _stats;

    public string Name => _name;
    public Stats Stats => _stats;

    public CharacterRace(string name, Stats stats)
    {
        _name = name;
        _stats = stats;
    }
    public CharacterRace(CharacterRaceTemplate selectedRace)
    {
        _name = selectedRace.Name;
        _stats = selectedRace.Stats;
    }

}
