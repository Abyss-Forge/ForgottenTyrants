using System.Collections;
using System.Collections.Generic;
using Systems.FSM;
using Systems.GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;

public class TotalDarknessAbility : AbilityStateMachine
{
    #region Specific ability properties

    //TODO

    #endregion
    #region Setup

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyState(this));  // estados especificos de esta habilidad
        _fsm.Add(new AbilityActiveState(this));
        _fsm.Add(new AbilityCooldownState(this));   // estados predeterminados
        _fsm.Add(new AbilityLockedState(this));
    }

    #endregion
    #region States

    public class AbilityReadyState : State<EAbilityState>
    {
        TotalDarknessAbility _ability;
        public AbilityReadyState(TotalDarknessAbility ability) : base(EAbilityState.READY)
        {
            _ability = ability;
        }

        private void OnCast(InputAction.CallbackContext context)
        {
            if (context.performed) _ability._fsm.TransitionTo(EAbilityState.ACTIVE);
        }

        public override void Enter()
        {
            base.Enter();

            MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_3, OnCast, true);
        }

        public override void Exit()
        {
            base.Exit();
            MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_3, OnCast, false);
        }
    }

    public class AbilityActiveState : State<EAbilityState>
    {
        TotalDarknessAbility _ability;
        public AbilityActiveState(TotalDarknessAbility ability) : base(EAbilityState.ACTIVE)
        {
            _ability = ability;
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