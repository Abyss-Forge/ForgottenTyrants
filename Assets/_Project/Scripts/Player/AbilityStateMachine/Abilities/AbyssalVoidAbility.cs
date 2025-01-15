using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbyssalVoidAbility : AbilityStateMachine
{
    #region Specific ability properties

    //TODO

    #endregion
    #region States

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyBaseState<AbyssalVoidAbility>(this, EAbilityState.READY));
        _fsm.Add(new AbilityActiveBaseState<AbyssalVoidAbility>(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<AbyssalVoidAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<AbyssalVoidAbility>(this, EAbilityState.LOCKED));
    }

    //TODO

    #endregion
}