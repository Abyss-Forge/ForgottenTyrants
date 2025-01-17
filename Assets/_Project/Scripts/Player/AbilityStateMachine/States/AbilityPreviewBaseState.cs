using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPreviewBaseState<T> : AbilityState<T> where T : AbilityStateMachine
{
    public AbilityPreviewBaseState(T ability, EAbilityState id) : base(ability, id)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _ability.FSM.TransitionTo(EAbilityState.ACTIVE);
    }

}