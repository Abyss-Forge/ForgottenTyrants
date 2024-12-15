using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected string _name;
    [SerializeField] protected Stats _stats = new(0, 0, 0, 0f, 0f, 0, 0, 0f);
    protected int _currentHp = 0;

    public string Name => _name;
    public Stats Stats => _stats;

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