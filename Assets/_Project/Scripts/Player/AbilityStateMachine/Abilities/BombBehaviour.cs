using System.Collections;
using System.Collections.Generic;
using Systems.FSM;
using UnityEngine;

public class BombBehaviour : ExplosiveProjectile
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
        _fsm.Add(new ProjectileLiveState(this));
        _fsm.Add(new ProjectileExplodingState(this));
        _fsm.Add(new ProjectileDestroyedState(this));
    }

    #endregion

    private class ProjectileLiveState : State<EProjectileState>
    {
        readonly Projectile _projectile;
        public ProjectileLiveState(Projectile projectile) : base(EProjectileState.LIVE) => _projectile = projectile;

        public override void Enter()
        {
            base.Enter();

            _projectile.gameObject.SetActive(true);
        }
    }

    private class ProjectileExplodingState : State<EProjectileState>
    {
        readonly Projectile _projectile;
        public ProjectileExplodingState(Projectile projectile) : base(EProjectileState.HIT) => _projectile = projectile;


    }

    private class ProjectileDestroyedState : State<EProjectileState>
    {
        readonly Projectile _projectile;
        public ProjectileDestroyedState(Projectile projectile) : base(EProjectileState.DESTROYED) => _projectile = projectile;

    }

}