using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;
using Utils.Extensions;

public class LivingChainsAbility : AbilityStateMachine, IAbilityWithRange
{
    #region Specific ability properties

    [SerializeField] private LineRenderer _lineRendererTemplate;

    private Dictionary<Transform, LineRenderer> _playerChains = new();

    #endregion
    #region Interface implementation

    [SerializeField] float _range;
    public float Range => _range;

    #endregion
    #region States

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyBaseState<LivingChainsAbility>(this, EAbilityState.READY));
        _fsm.Add(new AbilityPreviewBaseState<LivingChainsAbility>(this, EAbilityState.PREVIEW));
        _fsm.Add(new AbilityActiveState(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<LivingChainsAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<LivingChainsAbility>(this, EAbilityState.LOCKED));
    }

    private class AbilityActiveState : AbilityActiveBaseState<LivingChainsAbility>
    {
        public AbilityActiveState(LivingChainsAbility ability, EAbilityState id) : base(ability, id)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _ability.TryDetectPlayersInRange();
        }

        public override void Update()
        {
            base.Update();

            _ability.UpdateChainsPositions();
        }

        public override void Exit()
        {
            base.Exit();

            _ability.ResetChains();
        }
    }
    #endregion

    private void TryDetectPlayersInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _range, Layer.Player);
        bool hasHit = false;

        foreach (Collider collider in hitColliders)
        {
            ApplyEnemyEffect(collider.transform);
            hasHit = true;
        }

        if (!hasHit) _fsm.TransitionTo(EAbilityState.COOLDOWN);
    }

    private void ApplyEnemyEffect(Transform target)
    {
        if (!_playerChains.ContainsKey(target))
        {
            GameObject chain = new("Chain");
            chain.transform.SetParent(transform);
            LineRenderer lr = chain.CopyComponent(_lineRendererTemplate);

            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, target.position);
            //StartCoroutine(AnimateChain(lr, target.position));

            _playerChains.Add(target, lr);
        }
    }

    private void UpdateChainsPositions()
    {
        foreach (var item in _playerChains)
        {
            LineRenderer lr = item.Value;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, item.Key.position);
        }
    }

    private void ResetChains()
    {
        foreach (var item in _playerChains)
        {
            Destroy(item.Value.gameObject);
        }
        _playerChains.Clear();
    }
}