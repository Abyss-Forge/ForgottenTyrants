using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Systems.FSM;

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
        _fsm.Add(new BombLiveState(this));
        _fsm.Add(new BombExplodingState(this));
        _fsm.Add(new BombDestroyedState(this));
    }

    #endregion

    private class BombLiveState : State<EProjectileState>
    {
        readonly BombBehaviour _bomb;
        public BombLiveState(BombBehaviour bomb) : base(EProjectileState.LIVE) => _bomb = bomb;

        public override void Enter()
        {
            base.Enter();

            _bomb.gameObject.SetActive(true);
        }
    }

    private class BombExplodingState : State<EProjectileState>
    {
        readonly BombBehaviour _bomb;
        public BombExplodingState(BombBehaviour bomb) : base(EProjectileState.HIT) => _bomb = bomb;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_bomb.transform.position, _bomb._explosionRadius);
        }


    }

    private class BombDestroyedState : State<EProjectileState>
    {
        readonly BombBehaviour _bomb;
        public BombDestroyedState(BombBehaviour bomb) : base(EProjectileState.DESTROYED) => _bomb = bomb;

        public override void Enter()
        {
            base.Enter();

            //Destroy(_bomb.gameObject);
            _bomb.gameObject.SetActive(false);
        }
    }

}