using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCooldownState : State<EAbilityState>
{
    AbilityStateMachine _ability;
    public AbilityCooldownState(AbilityStateMachine ability) : base(EAbilityState.COOLDOWN)
    {
        _ability = ability;
    }

    public override void Enter()
    {
        base.Enter();

        //_ability.AbilityIcon.OnEnterCooldown();

        Debug.Log(_ability._fsm.PreviousState.ID + " " + _ability._fsm.CurrentState.ID + " " + _ability.CooldownTimer);
        if (_ability._fsm.PreviousState.ID == EAbilityState.ACTIVE)
        {
            _ability.CooldownTimer = _ability.CooldownDuration;
            Debug.Log("me ejecuto y no deberia");
        }
    }

    public override void Update()
    {
        base.Update();

        UpdateCooldownTimer();
    }

    public override void Exit()
    {
        base.Exit();

        if (_ability._fsm.CurrentState.ID == EAbilityState.READY)
        {
            _ability.CooldownTimer = _ability.CooldownDuration;
            //_ability.AbilityIcon.OnExitCooldown();
        }
    }

    private void UpdateCooldownTimer()
    {
        if (_ability.CooldownTimer > 0)
        {
            _ability.CooldownTimer -= Time.deltaTime;

            //_ability.AbilityIcon.OnUpdateCooldown(_ability.CooldownTimer, _ability.CooldownDuration);
        }
        else
        {
            _ability._fsm.SetCurrentState(EAbilityState.READY);
        }
    }
}