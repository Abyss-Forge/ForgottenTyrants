using System.Collections;
using System.Collections.Generic;
using Systems.GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;

public class TotalDarknessAbility : AbilityStateMachine
{
    #region Specific ability properties

    //TODO

    #endregion
    #region States

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyBaseState<TotalDarknessAbility>(this, EAbilityState.READY));
        _fsm.Add(new AbilityActiveState(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<TotalDarknessAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<TotalDarknessAbility>(this, EAbilityState.LOCKED));
    }

    public class AbilityActiveState : AbilityState<TotalDarknessAbility>
    {
        public AbilityActiveState(TotalDarknessAbility ability, EAbilityState id) : base(ability, id)
        {
        }

        private GhostStatusEffect ghostStatusEffect;

        private void OnCast(InputAction.CallbackContext context)
        {
            if (context.performed) _ability._fsm.TransitionTo(EAbilityState.COOLDOWN);
        }

        public override void Enter()
        {
            base.Enter();

            MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_3, OnCast, true);

            _ability.ActiveTimer = _ability.ActiveDuration;

            //_ability.AbilityIcon.OnEnterActive();

            ghostStatusEffect = new();
            ghostStatusEffect.ApplyEffect(_ability.gameObject.GetComponent<Player>());
        }

        public override void Update()
        {
            base.Update();

            UpdateActiveTimer();
        }

        public override void Exit()
        {
            base.Exit();

            MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_3, OnCast, false);

            ghostStatusEffect.RemoveEffect(_ability.gameObject.GetComponent<Player>());
        }

        private void UpdateActiveTimer()
        {
            if (_ability.ActiveTimer > 0)
            {
                _ability.ActiveTimer -= Time.deltaTime;
            }
            else
            {
                _ability._fsm.TransitionTo(EAbilityState.COOLDOWN);
            }
        }
    }

    #endregion
}