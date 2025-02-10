using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Entity : NetworkBehaviour
{
    [field: SerializeField] public string Name { get; protected set; }
    [SerializeField] protected Stats _baseStats = new();
    protected Stats _modifiedStats = new();
    public Stats BaseStats => _baseStats;
    public Stats ModifiedStats => _modifiedStats;

    public void AppyStatsModifier(Stats stats, bool isAppliedToBase)
    {

    }

    public float CurrentHp = 0;   // mierdas de diego
    void Start()
    {
        CurrentHp = BaseStats.Health;
    }

}