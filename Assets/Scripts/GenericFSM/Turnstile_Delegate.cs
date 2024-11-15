using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//clase de ejemplo
public class Turnstile_Delegate : MonoBehaviour
{
    private FiniteStateMachine<EState> _fsm = new();

    private enum EState
    {
        LOCKED,
        UNLOCKED,
    }

    private void Start()
    {
        _fsm.Add(new State<EState>(EState.LOCKED, "Locked", OnEnterLocked, null, OnUpdateLocked, null, null));
        _fsm.Add(new State<EState>(EState.UNLOCKED, "Unlocked", OnEnterUnlocked, null, OnUpdateUnlocked, null, null));

        _fsm.SetCurrentState(EState.LOCKED);
    }

    private void Update()
    {
        _fsm.Update();
    }

    private void FixedUpdate()
    {
        _fsm.FixedUpdate();
    }

    private void LateUpdate()
    {
        _fsm.LateUpdate();
    }

    #region Delegates implementation for the states.
    void OnEnterLocked()
    {
        Debug.Log("Turnstile LOCKED");
    }

    void OnUpdateLocked()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Turnstile unlocking");
                _fsm.SetCurrentState(EState.UNLOCKED);
            }
        }
    }

    void OnEnterUnlocked()
    {
        Debug.Log("Turnstile UNLOCKED.");
    }

    void OnUpdateUnlocked()
    {
        if (Input.anyKeyDown)
        {
            if (!Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Turnstile locking");
                _fsm.SetCurrentState(EState.LOCKED);
            }
        }
    }
    #endregion
}