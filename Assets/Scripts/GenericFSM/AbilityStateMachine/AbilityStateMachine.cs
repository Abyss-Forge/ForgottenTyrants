using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAbilityState
{
    READY, ACTIVE, COOLDOWN, LOCKED
}

public abstract class AbilityStateMachine : MonoBehaviour
{
    #region Default logic

    [field: SerializeField] public float ActiveDuration { get; private set; } = 5f;
    [field: SerializeField] public float CooldownDuration { get; private set; } = 5f;

    public float ActiveTimer { get; set; }
    public float CooldownTimer { get; set; }

    public void Lock(float time = -1) => StartCoroutine(ApplyLock(time));
    public void Unlock() => _fsm.SetCurrentState(EAbilityState.COOLDOWN);   //si el cooldown es 0, automaticamente transicionara a READY

    private IEnumerator ApplyLock(float time)
    {
        if (_fsm.GetCurrentState().ID != EAbilityState.ACTIVE)
        {
            _fsm.SetCurrentState(EAbilityState.LOCKED);

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

    #endregion
    #region Setup

    public FiniteStateMachine<EAbilityState> _fsm { get; private set; }

    void Awake()
    {
        ActiveTimer = ActiveDuration;
        CooldownTimer = CooldownDuration;
    }

    void Start()    //esto va en Start en vez de Awake pq usamos el MyInputManager que es un singleton
    {
        _fsm = new();
        InitializeStates();
        _fsm.SetCurrentState(EAbilityState.READY);
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

    protected virtual void InitializeStates()
    {
        // _fsm.Add(new AbilityReadyState(this));
        // _fsm.Add(new AbilityActiveState(this));
        // _fsm.Add(new AbilityCooldownState(this));
        // _fsm.Add(new AbilityLockedState(this));
    }

    #endregion
}