using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;

public class DeadlyPoisonAbility : AbilityStateMachine, IAbilityWithTarget, IAbilityWithDotTick, IAbilityWithBuff
{
    #region Specific ability properties


    #endregion
    #region Interface implementation

    private GameObject _target;
    GameObject IAbilityWithTarget.Target => _target;

    [SerializeField] private float _dotThreshold = 5f;
    float IAbilityWithDotTick.DotThreshold => _dotThreshold;

    [SerializeField] private Stats _statModifier;
    Stats IAbilityWithBuff.StatModifier => _statModifier;

    #endregion
    #region States

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyBaseState<DeadlyPoisonAbility>(this, EAbilityState.READY));
        _fsm.Add(new AbilityActiveState(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<DeadlyPoisonAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<DeadlyPoisonAbility>(this, EAbilityState.LOCKED));
    }

    private class AbilityActiveState : AbilityActiveBaseState<DeadlyPoisonAbility>
    {
        public AbilityActiveState(DeadlyPoisonAbility ability, EAbilityState id) : base(ability, id)
        {
        }

        private float _timer;

        override public void Enter()
        {
            base.Enter();

            TryGetTarget();
            _timer = 0f;
            ApplyDamageBuff();
        }

        override public void Update()
        {
            base.Update();

            ApplyPoison();
        }

        override public void Exit()
        {
            base.Exit();

            RemoveDamageBuff();
        }

        private void TryGetTarget()
        {
            _ability._target = CrosshairRaycaster.GetImpactObject();
            if (_ability._target == null || !_ability._target.CompareTag(Tag.Enemy))
            {
                _ability._fsm.TransitionTo(EAbilityState.COOLDOWN);
            }
        }

        private void ApplyPoison()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                Debug.Log($"Dot poison {_ability.Stats.MagicalDamage} dmg from {_ability._target.name} at {System.DateTime.Now}");
                _timer = _ability._dotThreshold;
            }
        }

        private void ApplyDamageBuff()
        {
            Debug.Log("Buffing damage for: " + _ability.Stats.PhysicalDamage);
        }

        private void RemoveDamageBuff()
        {
            Debug.Log("Debuffing damage for: " + _ability.Stats.PhysicalDamage);
        }
    }

    #endregion
}