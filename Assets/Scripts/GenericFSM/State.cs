﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<EState> where EState : Enum
{
    public string Name { get; set; }
    public EState ID { get; private set; }

    public State(EState id)
    {
        ID = id;
    }

    public State(EState id, string name) : this(id)
    {
        Name = name;
    }

    public delegate void DelegateNoArg();

    public DelegateNoArg OnEnter;
    public DelegateNoArg OnExit;
    public DelegateNoArg OnUpdate;
    public DelegateNoArg OnFixedUpdate;
    public DelegateNoArg OnLateUpdate;

    public State(EState id,
        DelegateNoArg onEnter,
        DelegateNoArg onExit = null,
        DelegateNoArg onUpdate = null,
        DelegateNoArg onFixedUpdate = null,
        DelegateNoArg onLateUpdate = null) : this(id)
    {
        OnEnter = onEnter;
        OnExit = onExit;
        OnUpdate = onUpdate;
        OnFixedUpdate = onFixedUpdate;
        OnLateUpdate = onLateUpdate;
    }
    public State(EState id,
        string name,
        DelegateNoArg onEnter,
        DelegateNoArg onExit = null,
        DelegateNoArg onUpdate = null,
        DelegateNoArg onFixedUpdate = null,
        DelegateNoArg onLateUpdate = null) : this(id, name)
    {
        OnEnter = onEnter;
        OnExit = onExit;
        OnUpdate = onUpdate;
        OnFixedUpdate = onFixedUpdate;
        OnLateUpdate = onLateUpdate;
    }

    virtual public void Enter()
    {
        OnEnter?.Invoke();
    }

    virtual public void Exit()
    {
        OnExit?.Invoke();
    }

    virtual public void Update()
    {
        OnUpdate?.Invoke();
    }

    virtual public void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }

    virtual public void LateUpdate()
    {
        OnLateUpdate?.Invoke();
    }

}