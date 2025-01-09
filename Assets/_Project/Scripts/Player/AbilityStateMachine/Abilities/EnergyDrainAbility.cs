using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForgottenTyrants;

public class EnergyDrainAbility : AbilityStateMachine, IAbilityWithTarget
{
    #region Specific ability properties

    [SerializeField] private float _dotThreshold = 5f, _dotDamage = 3f;

    #endregion
    #region Interface implementation

    [SerializeField] GameObject _target;
    GameObject IAbilityWithTarget.Target => _target;

    #endregion
    #region States

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyState(this, EAbilityState.READY));
        _fsm.Add(new AbilityActiveState(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<EnergyDrainAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<EnergyDrainAbility>(this, EAbilityState.LOCKED));
    }

    public class AbilityReadyState : AbilityState<EnergyDrainAbility>
    {
        public AbilityReadyState(EnergyDrainAbility ability, EAbilityState id) : base(ability, id)
        {
        }

        public override void Enter()
        {
            base.Enter();

            TryGetTarget();
        }

        private void TryGetTarget()
        {
            _ability._target = MyCursorManager.Instance.GetCrosshairImpactObject();
            if (_ability._target.CompareTag(Tag.Enemy))
            {
                _ability._fsm.TransitionTo(EAbilityState.ACTIVE);
            }
        }
    }

    public class AbilityActiveState : AbilityState<EnergyDrainAbility>
    {
        public AbilityActiveState(EnergyDrainAbility ability, EAbilityState id) : base(ability, id)
        {
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
                _ability._fsm.TransitionTo(EAbilityState.COOLDOWN);
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