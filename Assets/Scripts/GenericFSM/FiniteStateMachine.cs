using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine<EState> where EState : Enum
{
    protected Dictionary<EState, State<EState>> _states;
    protected State<EState> _currentState, _previousState;
    public State<EState> CurrentState => _currentState;
    public State<EState> PreviousState => _previousState;

    public delegate void DelegateWithState(EState oldState, EState newState);
    private DelegateWithState OnStateChange;

    public void SubscribeOnStateChange(DelegateWithState function, bool subscribe = true)
    {
        if (subscribe) OnStateChange += function;
        else OnStateChange -= function;
    }

    public FiniteStateMachine()
    {
        _states = new();
    }

    public void Add(State<EState> state) => _states.Add(state.ID, state);
    public void Add(State<EState> state, EState stateID) => _states.Add(stateID, state);

    public State<EState> GetState(EState stateID) => _states.ContainsKey(stateID) ? _states[stateID] : null;

    public void SetCurrentState(EState stateID) => SetCurrentState(_states[stateID]);
    public void SetCurrentState(State<EState> state)
    {
        if (_currentState == state)
        {
            return;
        }

        _previousState = _currentState;
        _currentState = state;

        OnStateChange?.Invoke(_previousState.ID, _currentState.ID);

        _previousState?.Exit();
        _currentState?.Enter();
    }

    public void Update() => _currentState?.Update();
    public void FixedUpdate() => _currentState?.FixedUpdate();
    public void LateUpdate() => _currentState?.LateUpdate();

}