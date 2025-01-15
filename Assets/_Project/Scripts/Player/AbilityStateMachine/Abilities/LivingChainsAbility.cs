using System.Collections.Generic;
using UnityEngine;
using ForgottenTyrants;
using Utils.Extensions;

public class LivingChainsAbility : AbilityStateMachine, IAbilityWithRange
{
    #region Specific ability properties

    [SerializeField] private LineRenderer _lineRendererTemplate;

    Dictionary<GameObject, GameObject> _playerChains = new();

    #endregion
    #region Interface implementation

    [SerializeField] float _range;
    float IAbilityWithRange.Range => _range;

    void IAbilityWithRange.OnDrawGizmos() => OnDrawGizmos();
    void OnDrawGizmos()
    {
        if (_fsm != null && _fsm.CurrentState.ID == EAbilityState.ACTIVE)
        {
            Gizmos.color = new(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, _range);
        }
    }

    #endregion
    #region States

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyBaseState<LivingChainsAbility>(this, EAbilityState.READY));
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

            TryDetectPlayersInRange();
        }

        public override void Update()
        {
            base.Update();

            UpdateChainsPositions();
        }

        public override void Exit()
        {
            base.Exit();

            ResetChains();
        }

        private void TryDetectPlayersInRange()
        {
            Collider[] hitColliders = Physics.OverlapSphere(_ability.transform.position, _ability._range, Layer.Player);
            bool hasHit = false;

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.CompareTag(Tag.Ally))
                {
                    ApplyAllyEffect(hitCollider.gameObject);
                    hasHit = true;
                }
                else if (hitCollider.gameObject.CompareTag(Tag.Enemy))
                {
                    ApplyEnemyEffect(hitCollider.gameObject);
                    hasHit = true;
                }
            }

            if (!hasHit) _ability._fsm.TransitionTo(EAbilityState.COOLDOWN);
        }

        private void ApplyAllyEffect(GameObject ally)
        {
            Debug.Log("Ally hit!: " + ally.name);
        }

        private void ApplyEnemyEffect(GameObject enemy)
        {
            if (!_ability._playerChains.ContainsKey(enemy))
            {
                GameObject chain = new("Chain");
                chain.transform.SetParent(_ability.transform);
                LineRenderer lr = chain.CopyComponent(_ability._lineRendererTemplate);

                lr.SetPosition(0, _ability.transform.position);
                lr.SetPosition(1, enemy.transform.position);
                //StartCoroutine(AnimateChain(lr, enemy.transform.position));

                _ability._playerChains.Add(enemy, chain);
            }
        }

        private void UpdateChainsPositions()
        {
            foreach (var item in _ability._playerChains)
            {
                LineRenderer lineRenderer = item.Value.GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, _ability.transform.position);
                lineRenderer.SetPosition(1, item.Key.transform.position);
            }
        }

        private void ResetChains()
        {
            foreach (var item in _ability._playerChains)
            {
                Destroy(item.Value);
            }
            _ability._playerChains.Clear();
        }
    }

    #endregion
}