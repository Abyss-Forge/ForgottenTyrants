using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForgottenTyrants;

public class EnergyDrainAbility : AbilityStateMachine, IAbilityWithTarget, IAbilityWithDotTick
{
    #region Specific ability properties


    #endregion
    #region Interface implementation

    private GameObject _target;
    public GameObject Target => _target;

    [SerializeField] private float _dotThreshold = 5f;
    public float DotThreshold => _dotThreshold;

    #endregion
    #region States

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyBaseState<EnergyDrainAbility>(this, EAbilityState.READY));
        _fsm.Add(new AbilityPreviewBaseState<EnergyDrainAbility>(this, EAbilityState.PREVIEW));
        _fsm.Add(new AbilityActiveState(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<EnergyDrainAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<EnergyDrainAbility>(this, EAbilityState.LOCKED));
    }

    private class AbilityActiveState : AbilityActiveBaseState<EnergyDrainAbility>
    {
        public AbilityActiveState(EnergyDrainAbility ability, EAbilityState id) : base(ability, id)
        {
        }

        private GhostStatusEffect _ghostStatusEffect;
        private float _timer;

        public override void Enter()
        {
            TryGetTarget();

            base.Enter();

            _ghostStatusEffect = new();
            _ghostStatusEffect.ApplyEffect(_ability.GetComponentInParent<Player>()); //TODO with service locator
            _timer = 0;
        }

        public override void Update()
        {
            base.Update();

            ApplyDamageAbsorptionEffect();
        }

        public override void Exit()
        {
            base.Exit();

            _ghostStatusEffect.RemoveEffect(_ability.GetComponentInParent<Player>());
        }

        private void TryGetTarget()
        {
            _ability._target = CrosshairRaycaster.GetImpactObject();
            if (_ability._target == null || !_ability._target.CompareTag(Tag.Enemy))
            {
                _ability._fsm.TransitionTo(EAbilityState.COOLDOWN);
            }
        }

        private void ApplyDamageAbsorptionEffect()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                Debug.Log($"Absorbing {_ability.Stats.MagicalDamage} dmg from {_ability._target.name} at {System.DateTime.Now}");
                _timer = _ability._dotThreshold;
            }
        }
    }

    #endregion
}