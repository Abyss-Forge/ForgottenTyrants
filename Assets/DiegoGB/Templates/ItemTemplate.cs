using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemTemplate : ScriptableObject
{
    [SerializeField] protected Stats _stats;
    [SerializeField] protected string _name;

    public Stats Stats => _stats;
    public string Name => _name;
}
