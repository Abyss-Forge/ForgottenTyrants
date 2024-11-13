using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine<EState> where EState : Enum
{
    protected Dictionary<EState, State<EState>> _states;
    protected State<EState> _currentState;

    public FiniteStateMachine()
    {
        _states = new Dictionary<EState, State<EState>>();
    }

    public void Add(State<EState> state)
    {
        _states.Add(state.ID, state);
    }

    public void Add(State<EState> state, EState stateID)
    {
        _states.Add(stateID, state);
    }

    public State<EState> GetState(EState stateID)
    {
        if (_states.ContainsKey(stateID))
        {
            return _states[stateID];
        }
        return null;
    }

    public void SetCurrentState(EState stateID)
    {
        State<EState> state = _states[stateID];
        SetCurrentState(state);
    }

    public State<EState> GetCurrentState()
    {
        return _currentState;
    }

    public void SetCurrentState(State<EState> state)
    {
        if (_currentState == state)
        {
            return;
        }

        if (_currentState != null)
        {
            _currentState.Exit();
        }

        _currentState = state;

        if (_currentState != null)
        {
            _currentState.Enter();
        }
    }

    public void Update()
    {
        if (_currentState != null)
        {
            _currentState.Update();
        }
    }

    public void FixedUpdate()
    {
        if (_currentState != null)
        {
            _currentState.FixedUpdate();
        }
    }

    public void LateUpdate()
    {
        if (_currentState != null)
        {
            _currentState.LateUpdate();
        }
    }

}