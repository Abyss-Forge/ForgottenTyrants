using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForgottenTyrants;
using Systems.FSM;

public class FireballBehaviour : ExplosiveProjectile
{
    #region Setup

    public FiniteStateMachine<EProjectileState> _fsm { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _fsm = new();
        InitializeStates();
        _fsm.TransitionTo(EProjectileState.LIVE);
    }

    protected override void Update()
    {
        base.Update();
        _fsm.Update();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        _fsm.FixedUpdate();
    }
    void LateUpdate() => _fsm.LateUpdate();

    protected virtual void InitializeStates()
    {
        _fsm.Add(new FireballLiveState(this));
        _fsm.Add(new FireballHitState(this));
        _fsm.Add(new FireballDestroyedState(this));
    }

    #endregion

    private class FireballLiveState : State<EProjectileState>
    {
        readonly FireballBehaviour _projectile;
        public FireballLiveState(FireballBehaviour projectile) : base(EProjectileState.LIVE) => _projectile = projectile;

        public override void Enter()
        {
            base.Enter();

            _projectile.gameObject.SetActive(true);
        }

        public override void Update()
        {
            base.Update();

            ScaleUp();
        }

        private void ScaleUp()
        {
            _projectile.transform.localScale += Vector3.one * Time.deltaTime;
        }
    }

    private class FireballHitState : State<EProjectileState>
    {
        readonly FireballBehaviour _projectile;
        public FireballHitState(FireballBehaviour projectile) : base(EProjectileState.HIT) => _projectile = projectile;

    }

    private class FireballDestroyedState : State<EProjectileState>
    {
        readonly FireballBehaviour _projectile;
        public FireballDestroyedState(FireballBehaviour projectile) : base(EProjectileState.DESTROYED) => _projectile = projectile;

        public override void Enter()
        {
            base.Enter();

            //Destroy(_bomb.gameObject);
            _projectile.gameObject.SetActive(false);
        }
    }

}