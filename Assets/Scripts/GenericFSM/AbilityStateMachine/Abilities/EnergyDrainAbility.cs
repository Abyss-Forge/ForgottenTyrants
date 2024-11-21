using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ForgottenTyrants;

public class EnergyDrainAbility : AbilityStateMachine
{
    #region Specific ability properties

    [SerializeField] private float _dotThreshold = 5f, _dotDamage = 3f;

    private GameObject _target;

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
        EnergyDrainAbility _ability;
        public AbilityReadyState(EnergyDrainAbility ability) : base(EAbilityState.READY)
        {
            _ability = ability;
        }

        private void OnCast(InputAction.CallbackContext context)
        {
            if (context.performed) TargetEnemy();
        }

        public override void Enter()
        {
            base.Enter();
            MyInputManager.Instance.SubscribeToInput(EInputAction.CLASS_ABILITY_2, OnCast, true);
        }

        public override void Exit()
        {
            base.Exit();
            MyInputManager.Instance.SubscribeToInput(EInputAction.CLASS_ABILITY_2, OnCast, false);
        }

        private void TargetEnemy()
        {
            _ability._target = MyCursorManager.Instance.GetCrosshairTarget();
            if (_ability._target.CompareTag(Tag.Enemy))
            {
                _ability._fsm.SetCurrentState(EAbilityState.ACTIVE);
            }
        }
    }

    public class AbilityActiveState : State<EAbilityState>
    {
        EnergyDrainAbility _ability;
        public AbilityActiveState(EnergyDrainAbility ability) : base(EAbilityState.ACTIVE)
        {
            _ability = ability;
        }

        private GhostStatusEffect ghostStatusEffect;
        private float timer;

        public override void Enter()
        {
            base.Enter();

            _ability.ActiveTimer = _ability.ActiveDuration;

            //_ability.AbilityIcon.OnEnterActive();

            ghostStatusEffect = new();
            ghostStatusEffect.ApplyEffect(_ability.gameObject.GetComponent<Player>());
            timer = 0;
        }

        public override void Exit()
        {
            base.Exit();

            ghostStatusEffect.RemoveEffect(_ability.gameObject.GetComponent<Player>());
        }

        public override void Update()
        {
            base.Update();

            UpdateActiveTimer();
            ApplyDamageAbsorptionEffect();
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

        private void ApplyDamageAbsorptionEffect()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Debug.Log($"Absorbing {_ability._dotDamage} dmg from {_ability._target.name} at {System.DateTime.Now}");
                timer = _ability._dotThreshold;
            }
        }
    }

    #endregion
}