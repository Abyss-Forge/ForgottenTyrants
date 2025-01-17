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
        _fsm.Add(new AbilityActiveState(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<TotalDarknessAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<TotalDarknessAbility>(this, EAbilityState.LOCKED));
    }

    private class AbilityActiveState : AbilityActiveBaseState<TotalDarknessAbility>
    {
        public AbilityActiveState(TotalDarknessAbility ability, EAbilityState id) : base(ability, id)
        {
        }

        private GhostStatusEffect _ghostStatusEffect;

        public override void Enter()
        {
            base.Enter();

            _ghostStatusEffect = new();
            _ghostStatusEffect.ApplyEffect(_ability.gameObject.GetComponent<Player>());
        }

        public override void Exit()
        {
            base.Exit();

            _ghostStatusEffect.RemoveEffect(_ability.gameObject.GetComponent<Player>());
        }
    }

    #endregion
}