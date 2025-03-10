using System.Collections;
using System.Collections.Generic;
using Systems.FSM;
using UnityEngine;

public class AbilityLockedBaseState<T> : AbilityState<T> where T : AbilityStateMachine
{
    public AbilityLockedBaseState(T ability, EAbilityState id) : base(ability, id)
    {
    }

    public override void Update()
    {
        base.Update();

        UpdateCooldownTimer();  //importante hacer que si se lockea al entrar en cooldown, el cooldown siga rulando, si se lockea en ready no pase nada especial, y que si se lockea en active, el active se reinicie igual (no se deberia poder cortar una habilidad en mi opinion)
    }

    public override void Enter()
    {
        base.Enter();

        //_ability.AbilityIcon.OnEnterLocked();
    }

    public override void Exit()
    {
        base.Exit();

        //_ability.AbilityIcon.OnExitLocked();
    }

    private void UpdateCooldownTimer()
    {
        if (_ability.CooldownTimer > 0)
        {
            _ability.CooldownTimer -= Time.deltaTime;

            //_ability.AbilityIcon.OnUpdateCooldown(_ability.CooldownTimer, _ability.CooldownDuration);
        }
    }
}