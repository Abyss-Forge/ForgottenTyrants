using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalDarknessAbility : AbilityStateMachine
{
    #region Specific ability properties

    //TODO

    #endregion
    #region States

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyBaseState<TotalDarknessAbility>(this, EAbilityState.READY));
        _fsm.Add(new AbilityPreviewBaseState<TotalDarknessAbility>(this, EAbilityState.PREVIEW));
        _fsm.Add(new AbilityActiveBaseState<TotalDarknessAbility>(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<TotalDarknessAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<TotalDarknessAbility>(this, EAbilityState.LOCKED));
    }

    #endregion
}