using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turnstile_Class : MonoBehaviour
{
    private FiniteStateMachine<EState> _fsm = new();

    public enum EState
    {
        LOCKED,
        UNLOCKED,
    }

    public class TurnstileLocked : State<EState>
    {
        Turnstile_Class _turnstile;
        public TurnstileLocked(Turnstile_Class turnstile) : base(EState.LOCKED)
        {
            _turnstile = turnstile;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Turnstile LOCKED. Press C key to insert a coin");
        }

        public override void Update()
        {
            base.Update();
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log("Turnstile unlocking");
                    _turnstile._fsm.SetCurrentState(EState.UNLOCKED);
                }
                else
                {
                    Debug.Log("Incorrect coin");
                }
            }
        }
    }

    public class TurnstileUnlocked : State<EState>
    {
        Turnstile_Class _turnstile;
        public TurnstileUnlocked(Turnstile_Class turnstile) : base(EState.UNLOCKED)
        {
            _turnstile = turnstile;
        }

        public override void Enter()
        {
            Debug.Log("Turnstile UNLOCKED. Press C key to close it");
            base.Enter();
        }

        public override void Update()
        {
            base.Update();
            if (Input.anyKeyDown)
            {
                if (!Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log("Turnstile locking");
                    _turnstile._fsm.SetCurrentState(EState.LOCKED);
                }
            }
        }
    }

    void Awake()
    {
        InitializeStates();
    }

    void Update()
    {
        _fsm.Update();
    }

    void FixedUpdate()
    {
        _fsm.FixedUpdate();
    }

    void LateUpdate()
    {
        _fsm.LateUpdate();
    }

    private void InitializeStates()
    {
        _fsm.Add(new TurnstileLocked(this));
        _fsm.Add(new TurnstileUnlocked(this));
        _fsm.SetCurrentState(EState.LOCKED);
    }

}