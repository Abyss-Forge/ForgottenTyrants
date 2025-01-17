using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Entity : NetworkBehaviour
{
    [field: SerializeField] public string Name { get; protected set; }
    protected Stats _baseStats = new(), _modifiedStats = new();
    public Stats BaseStats => _baseStats;
    public Stats ModifiedStats => _modifiedStats;

    public void AppyStatsModifier(Stats stats, bool isAppliedToBase)
    {

    }

    public int CurrentHp = 0;   // TODO: Remove this
    void Start()
    {
        CurrentHp = BaseStats.Hp;
    }

}