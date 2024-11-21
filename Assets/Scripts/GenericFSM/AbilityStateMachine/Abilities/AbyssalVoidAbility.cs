using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbyssalVoidAbility : AbilityStateMachine
{
    #region Specific ability properties

    void OnDrawGizmos()
    {
        if (_fsm != null && _fsm.CurrentState.ID == EAbilityState.ACTIVE)
        {
            Gizmos.color = new(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, 30);
        }
    }

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
        AbyssalVoidAbility _ability;
        public AbilityReadyState(AbyssalVoidAbility ability) : base(EAbilityState.READY)
        {
            _ability = ability;
        }

        private void OnCast(InputAction.CallbackContext context)
        {
            if (context.performed) _ability._fsm.SetCurrentState(EAbilityState.ACTIVE);
        }

        public override void Enter()
        {
            base.Enter();
            MyInputManager.Instance.SubscribeToInput(EInputAction.CLASS_ABILITY_4, OnCast, true);
        }

        public override void Exit()
        {
            base.Exit();
            MyInputManager.Instance.SubscribeToInput(EInputAction.CLASS_ABILITY_4, OnCast, false);
        }
    }

    public class AbilityActiveState : State<EAbilityState>
    {
        AbyssalVoidAbility _ability;
        public AbilityActiveState(AbyssalVoidAbility ability) : base(EAbilityState.ACTIVE)
        {
            _ability = ability;
        }

        public override void Enter()
        {
            base.Enter();

            _ability.ActiveTimer = _ability.ActiveDuration;

            //_ability.AbilityIcon.OnEnterActive();
            Debug.Log("Abyssal Void triggered");
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
                _ability._fsm.SetCurrentState(EAbilityState.COOLDOWN);
            }
        }
    }

    #endregion
}