using System.Collections;
using System.Collections.Generic;
using Systems.FSM;
using UnityEngine;

public class AbilityCooldownBaseState<T> : AbilityState<T> where T : AbilityStateMachine
{
    public AbilityCooldownBaseState(T ability, EAbilityState id) : base(ability, id)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //_ability.AbilityIcon.OnEnterCooldown();

        if (_ability.FSM.PreviousState.ID == EAbilityState.ACTIVE)
        {
            _ability.CooldownTimer = _ability.CooldownDuration;
        }
    }

    public override void Update()
    {
        base.Update();

        UpdateCooldownTimer();
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
            _ability.FSM.TransitionTo(EAbilityState.READY);
        }
    }
}