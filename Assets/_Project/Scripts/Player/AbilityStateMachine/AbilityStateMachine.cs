using System.Collections;
using System.Collections.Generic;
using Systems.FSM;
using Systems.ServiceLocator;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EAbilityState
{
    READY, PREVIEW, ACTIVE, COOLDOWN, LOCKED
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

    protected List<NetworkedInfo> _networkedInfoList { get; } = new();

    public void Lock(float time = -1) => StartCoroutine(ApplyLock(time));
    public void Unlock() => _fsm.TransitionTo(EAbilityState.COOLDOWN);   //if cooldown is 0, will automatically transition to READY state

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

    public virtual void OnTriggered(InputAction.CallbackContext context)
    {
        if (context.performed) UpdateState();
    }

    protected virtual void UpdateState()
    {
        if (_fsm.CurrentState.ID == EAbilityState.READY)
        {
            CalculateInfo();
            AddInfo();
            _fsm.TransitionTo(EAbilityState.ACTIVE);
        }
        else if (_fsm.CurrentState.ID == EAbilityState.ACTIVE)
        {
            if (CanBeCanceled) ActiveTimer = 0;
        }
    }

    protected virtual void CalculateInfo()
    {
        ServiceLocator.Global.Get(out PlayerInfo player);

        if (Stats.PhysicalDamage <= 0)
        {
            float damage = player.Stats.PhysicalDamage + Stats.PhysicalDamage;
            _networkedInfoList.Add(new DamageInfo(
                teamId: player.Data.TeamId,
                affectedChannels: new() { EDamageApplyChannel.ENEMIES },
                damageAmount: damage,
                damageType: EElementalType.PHYSIC));
        }

        if (Stats.MagicalDamage <= 0)
        {
            float damage = player.Stats.MagicalDamage + Stats.MagicalDamage;
            _networkedInfoList.Add(new DamageInfo(
                teamId: player.Data.TeamId,
                affectedChannels: new() { EDamageApplyChannel.ENEMIES },
                damageAmount: damage,
                damageType: EElementalType.MAGIC));
        }

        if (Stats.Health <= 0)
        {
            float healAmount = Stats.Health;
            _networkedInfoList.Add(new HealInfo(
                teamId: player.Data.TeamId,
                affectedChannels: new() { EDamageApplyChannel.ALLIES },
                healAmount: healAmount));
        }
    }

    private void AddInfo()
    {
        InfoContainer container = gameObject.AddComponent<InfoContainer>();
        container.InfoList = _networkedInfoList;
    }

    #endregion
    #region Setup

    protected FiniteStateMachine<EAbilityState> _fsm;
    public FiniteStateMachine<EAbilityState> FSM => _fsm;

    void Awake()
    {
        _fsm = new();
        InitializeStates();
        _fsm.TransitionTo(EAbilityState.READY);
    }

    void Update() => _fsm.Update();
    void FixedUpdate() => _fsm.FixedUpdate();
    void LateUpdate() => _fsm.LateUpdate();

    protected abstract void InitializeStates();
    // _fsm.Add(new AbilityDefaultReadyState(this, EAbilityState.READY));
    // _fsm.Add(new AbilityDefaultActiveState(this, EAbilityState.ACTIVE));
    // _fsm.Add(new AbilityDefaultCooldownState(this, EAbilityState.COOLDOWN));
    // _fsm.Add(new AbilityDefaultLockedState(this, EAbilityState.LOCKED));

    #endregion
}