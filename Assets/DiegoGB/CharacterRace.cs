using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterRace
{
    [SerializeField] Stats raceStats;

    public Stats RaceStats => raceStats;

}
