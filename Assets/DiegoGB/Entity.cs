using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int _currentHp = 0;
    [SerializeField] protected string _name;
    [SerializeField] protected Stats _baseStats = new(), _modifiedStats = new();
    public string Name => _name;
    public Stats BaseStats => _baseStats;
    public Stats ModifiedStats => _modifiedStats;

    public void AppyStatsModifier(Stats stats, bool isAppliedToBase)
    {

    }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

}