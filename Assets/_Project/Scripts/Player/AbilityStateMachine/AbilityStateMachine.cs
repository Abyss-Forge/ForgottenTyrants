using System.Collections;
using System.Collections.Generic;
using Systems.FSM;
using UnityEngine;

public enum EAbilityState
{
    READY, ACTIVE, COOLDOWN, LOCKED
}

public abstract class AbilityStateMachine : MonoBehaviour, IAbilityBase
{
    #region Default logic

    [field: SerializeField] public float ActiveDuration { get; private set; } = 5f;
    [field: SerializeField] public float CooldownDuration { get; private set; } = 5f;

    public float ActiveTimer { get; set; }
    public float CooldownTimer { get; set; }

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

    }

    #endregion
    #region Setup

    public FiniteStateMachine<EAbilityState> _fsm { get; private set; }

    void Awake()
    {
        _fsm = new();
        InitializeStates();
        _fsm.TransitionTo(EAbilityState.READY);

        ActiveTimer = ActiveDuration;
        CooldownTimer = CooldownDuration;
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