using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterRace
{
    [SerializeField] Stats _stats;

    public Stats Stats => _stats;

}
