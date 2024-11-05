using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemTemplate : ScriptableObject
{
    [SerializeField] Stats _stats;
    [SerializeField] string _name;

    public Stats Stats => _stats;
    public string Name => _name;
}
