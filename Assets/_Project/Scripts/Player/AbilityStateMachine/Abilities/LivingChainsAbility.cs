using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ForgottenTyrants;
using Systems.FSM;
using Systems.GameManagers;

public class LivingChainsAbility : AbilityStateMachine
{
    #region Specific ability properties

    [SerializeField] private float _range = 10;
    [SerializeField] private LineRenderer _lineRendererTemplate;

    private Dictionary<GameObject, GameObject> _playerChains = new();

    void OnDrawGizmos()
    {
        if (_fsm != null && _fsm.CurrentState.ID == EAbilityState.ACTIVE)
        {
            Gizmos.color = new(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, _range);
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
        LivingChainsAbility _ability;
        public AbilityReadyState(LivingChainsAbility ability) : base(EAbilityState.READY)
        {
            _ability = ability;
        }

        private void OnCast(InputAction.CallbackContext context)
        {
            if (context.performed) DetectPlayersInRange();
        }

        public override void Enter()
        {
            base.Enter();
            MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_1, OnCast, true);
        }

        public override void Exit()
        {
            base.Exit();
            MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_1, OnCast, false);
        }

        private void DetectPlayersInRange()
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

            _ability._fsm.TransitionTo(hasHit ? EAbilityState.ACTIVE : EAbilityState.COOLDOWN);
        }

        private void ApplyAllyEffect(GameObject ally)
        {
            Debug.Log("Ally hit!: " + ally.name);
        }

        private void ApplyEnemyEffect(GameObject enemy)
        {
            if (!_ability._playerChains.ContainsKey(enemy))
            {
                GameObject chain = new();
                chain.transform.SetParent(_ability.transform);
                LineRenderer lr = chain.CopyComponent(_ability._lineRendererTemplate);
                chain.name = "Chain";

                lr.SetPosition(0, _ability.transform.position);
                lr.SetPosition(1, enemy.transform.position);
                //StartCoroutine(AnimateChain(lr, enemy.transform.position));

                _ability._playerChains.Add(enemy, chain);
            }
        }
    }

    public class AbilityActiveState : State<EAbilityState>
    {
        LivingChainsAbility _ability;
        public AbilityActiveState(LivingChainsAbility ability) : base(EAbilityState.ACTIVE)
        {
            _ability = ability;
        }

        public override void Enter()
        {
            base.Enter();

            _ability.ActiveTimer = _ability.ActiveDuration;

            //_ability.AbilityIcon.OnEnterActive();
        }

        public override void Update()
        {
            base.Update();

            UpdateActiveTimer();
            UpdateChainsPositions();
        }

        public override void Exit()
        {
            base.Exit();

            ResetChains();
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