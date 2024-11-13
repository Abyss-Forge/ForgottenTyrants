using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ability : MonoBehaviour
{

    [SerializeField]
    private float _cooldownDuration = 5, _abilityDuration = 5;

    private float _cooldownTimer = 0f, _activeTimer = 0f;

    /*private void UpdateCooldownTimer()
    {
        if (_fsm.GetCurrentState().ID == EState.COOLDOWN && _cooldownTimer > 0) _cooldownTimer -= Time.deltaTime;
    }*/




    //esto es especifico de esta habilidad
    [SerializeField] private LineRenderer _lineRendererTemplate;

    [SerializeField] private float _range = 10;

    private Dictionary<GameObject, GameObject> _playerChains = new();




    void OnDrawGizmos()
    {
        if (_fsm != null && _fsm.GetCurrentState().ID == EState.ACTIVE)
        {
            Gizmos.color = new(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, _range);
        }
    }






    #region StateMachine

    private FiniteStateMachine<EState> _fsm;
    public enum EState
    {
        READY, ACTIVE, COOLDOWN, LOCKED
    }

    #endregion
    #region States

    public class AbilityReady : State<EState>
    {
        Ability _ability;
        public AbilityReady(Ability ability) : base(EState.READY)
        {
            _ability = ability;
        }

        private void OnCast(InputAction.CallbackContext context)
        {
            if (context.performed) Cast();
        }

        public override void Enter()
        {
            base.Enter();

            MyInputManager.Instance.SubscribeToInput(EInputActions.ClassAbility1, OnCast, true);
        }

        public override void Exit()
        {
            base.Exit();

            MyInputManager.Instance.SubscribeToInput(EInputActions.ClassAbility1, OnCast, false);
        }

        private void ChangeState(EState state)
        {
            _ability._fsm.SetCurrentState(state);
        }

        private void Cast()
        {
            if (_ability._cooldownTimer <= 0f)
            {
                DetectPlayersInRange();
                _ability._cooldownTimer = _ability._cooldownDuration;
            }
        }

        private void DetectPlayersInRange()
        {
            Collider[] hitColliders = Physics.OverlapSphere(_ability.transform.position, _ability._range);

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.CompareTag(Tag.Ally))
                {
                    ApplyAllyEffect(hitCollider.gameObject);
                }
                else if (hitCollider.gameObject.CompareTag(Tag.Enemy))
                {
                    ApplyEnemyEffect(hitCollider.gameObject);
                }
            }

            ChangeState(EState.ACTIVE);
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

    public class AbilityActive : State<EState>
    {
        Ability _ability;
        public AbilityActive(Ability ability) : base(EState.ACTIVE)
        {
            _ability = ability;
        }

        private void ChangeState(EState state)
        {
            _ability._fsm.SetCurrentState(state);
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
            if (_ability._activeTimer > 0)
            {
                _ability._activeTimer -= Time.deltaTime;
            }
            else
            {
                _ability._activeTimer = _ability._abilityDuration; //esto no deberia causar conflicto ya que cambiamos de estado pero hay que probarlo
                ChangeState(EState.COOLDOWN);
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

    public class AbilityCooldown : State<EState>
    {
        Ability _ability;
        public AbilityCooldown(Ability ability) : base(EState.COOLDOWN)
        {
            _ability = ability;
        }

        private void ChangeState(EState state)
        {
            _ability._fsm.SetCurrentState(state);
        }

        public override void Update()
        {
            base.Update();

            UpdateCooldownTimer();
        }

        private void UpdateCooldownTimer()
        {
            if (_ability._cooldownTimer > 0)
            {
                _ability._cooldownTimer -= Time.deltaTime;
            }
            else
            {
                _ability._cooldownTimer = _ability._cooldownDuration;
                ChangeState(EState.READY);
            }
        }
    }

    public class AbilityLocked : State<EState>
    {
        Ability _ability;
        public AbilityLocked(Ability ability) : base(EState.LOCKED)
        {
            _ability = ability;
        }

        private void ChangeState(EState state)
        {
            _ability._fsm.SetCurrentState(state);
        }

        public override void Update()
        {
            base.Update();

            //TODO
        }
    }

    #endregion
    #region Setup

    void Awake()
    {
        InitializeStates();
    }

    void Update()
    {
        _fsm.Update();

        //UpdateCooldownTimer();
    }

    void FixedUpdate()
    {
        _fsm.FixedUpdate();
    }

    void LateUpdate()
    {
        _fsm.LateUpdate();
    }

    private void InitializeStates()
    {
        _fsm = new();

        _fsm.Add(new AbilityReady(this));
        _fsm.Add(new AbilityActive(this));
        _fsm.Add(new AbilityCooldown(this));
        _fsm.Add(new AbilityLocked(this));

        _fsm.SetCurrentState(EState.READY);
    }

    #endregion
}