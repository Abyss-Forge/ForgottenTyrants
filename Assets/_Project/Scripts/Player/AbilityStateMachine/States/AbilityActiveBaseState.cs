using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityActiveBaseState<T> : AbilityState<T> where T : AbilityStateMachine
{
    public AbilityActiveBaseState(T ability, EAbilityState id) : base(ability, id)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _ability.ActiveTimer = _ability.ActiveDuration;

        //_ability.AbilityIcon.OnEnterActive();
    }

    public override void Update()
    {
        base.Update();

        UpdateActiveTimer();
    }

    public override void Exit()
    {
        base.Exit();

    }

    private void UpdateActiveTimer()
    {
        if (_ability.ActiveTimer > 0)
        {
            _ability.ActiveTimer -= Time.deltaTime;
        }
        else
        {
            _ability.FSM.TransitionTo(EAbilityState.COOLDOWN);
        }
    }

}