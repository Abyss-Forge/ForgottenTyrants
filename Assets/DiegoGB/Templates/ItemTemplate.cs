using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemTemplate : ScriptableObject
{
    [SerializeField] protected string _uid;
    [SerializeField] protected Stats _stats;

    public string UID => _uid;
    public Stats Stats => _stats;

}