using System.Collections;
using System.Collections.Generic;
using Systems.FSM;
using Unity.Netcode;
using UnityEngine;

public enum EAbilityState
{
    READY, ACTIVE, COOLDOWN, LOCKED
}

public abstract class AbilityStateMachine : MonoBehaviour, IAbilityBase
{
    #region Default logic

    [field: SerializeField] public Stats Stats { get; private set; }

    [field: SerializeField] public Transform SpawnPoint { get; private set; }
    [field: SerializeField] public bool CanBeCanceled { get; private set; } = false;

    [field: SerializeField] public float ActiveDuration { get; private set; } = 5f;
    [field: SerializeField] public float CooldownDuration { get; private set; } = 5f;

    public float ActiveTimer { get; set; } = 0;
    public float CooldownTimer { get; set; } = 0;

    public void Lock(float time = -1) => StartCoroutine(ApplyLock(time));
    public void Unlock() => _fsm.TransitionTo(EAbilityState.COOLDOWN);   //si el cooldown es 0, automaticamente transicionara a READY

    private IEnumerator ApplyLock(float time)
    {
        if (_fsm.CurrentState.ID != EAbilityState.ACTIVE)
        {
            _fsm.TransitionTo(EAbilityState.LOCKED);

            if (time > 0)
            {
                while (time > 0)
                {
                    time -= Time.deltaTime;
                    yield return null;
                }
                Unlock();
            }
        }
        yield return null;
    }

    public void Trigger()
    {
        if (_fsm.CurrentState.ID == EAbilityState.READY)
        {
            _fsm.TransitionTo(EAbilityState.ACTIVE);
        }
        else if (_fsm.CurrentState.ID == EAbilityState.ACTIVE)
        {
            if (CanBeCanceled) ActiveTimer = 0;
        }
    }

    #endregion
    #region Setup

    public FiniteStateMachine<EAbilityState> _fsm { get; private set; }

    void Awake()
    {
        _fsm = new();
        InitializeStates();
        _fsm.TransitionTo(EAbilityState.READY);
    }

    void Update() => _fsm.Update();
    void FixedUpdate() => _fsm.FixedUpdate();
    void LateUpdate() => _fsm.LateUpdate();

    protected virtual void InitializeStates()
    {
        // _fsm.Add(new AbilityDefaultReadyState(this, EAbilityState.READY));
        // _fsm.Add(new AbilityDefaultActiveState(this, EAbilityState.ACTIVE));
        // _fsm.Add(new AbilityDefaultCooldownState(this, EAbilityState.COOLDOWN));
        // _fsm.Add(new AbilityDefaultLockedState(this, EAbilityState.LOCKED));
    }

    #endregion
}