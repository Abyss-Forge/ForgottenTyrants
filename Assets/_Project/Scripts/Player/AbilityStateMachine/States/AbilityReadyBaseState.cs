using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityReadyBaseState<T> : AbilityState<T> where T : AbilityStateMachine
{
    public AbilityReadyBaseState(T ability, EAbilityState id) : base(ability, id)
    {
    }

}