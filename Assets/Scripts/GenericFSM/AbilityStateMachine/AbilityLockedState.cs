using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLockedState : State<EAbilityState>
{
    AbilityStateMachine _ability;
    public AbilityLockedState(AbilityStateMachine ability) : base(EAbilityState.LOCKED)
    {
        _ability = ability;
    }

    public override void Update()
    {
        base.Update();

        UpdateCooldownTimer();  //importante hacer que si se lockea al entrar en cooldown, el cooldown siga rulandp, si se lockea en ready no pase nada especial, y que si se lockea en active, el active se reinicie igual (no se deberia poder cortar una habilidad en mi opinion)
    }

    private void UpdateCooldownTimer()
    {
        if (_ability.CooldownTimer > 0)
        {
            _ability.CooldownTimer -= Time.deltaTime;
        }
    }
}