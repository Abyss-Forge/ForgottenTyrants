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
        }
        else
        {
            _ability.CooldownTimer = _ability.CooldownDuration;   //esto no deberia causar conflicto ya que cambiamos de estado
            _ability._fsm.SetCurrentState(EAbilityState.READY);
        }
    }
}