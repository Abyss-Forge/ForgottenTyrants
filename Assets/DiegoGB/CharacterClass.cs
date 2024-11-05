using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterClass
{
    [SerializeField] Stats classStats;

    public Stats ClassStats => classStats;
}
