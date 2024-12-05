using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForgottenTyrants;

public class BombBehaviour : ExplosiveProjectile
{

    public enum EProjectileState
    {
        LIVE, HIT, DESTROYED
    }

    #region Setup

    public FiniteStateMachine<EProjectileState> _fsm { get; private set; }

    /*void Awake()
    {
        _fsm = new();
        InitializeStates();
        _fsm.SetCurrentState(EProjectileState.LIVE);
    }

    void Update() => _fsm.Update();
    void FixedUpdate() => _fsm.FixedUpdate();
    void LateUpdate() => _fsm.LateUpdate();

    protected virtual void InitializeStates()
    {
        _fsm.Add(new BombLiveState(this));
        _fsm.Add(new BombExplodingState(this));
        _fsm.Add(new BombDestroyedState(this));
    }*/

    #endregion

    public class BombLiveState : State<EProjectileState>
    {
        readonly BombBehaviour _bomb;
        public BombLiveState(BombBehaviour bomb) : base(EProjectileState.LIVE) => _bomb = bomb;

        void OnDrawGizmos()
        {
            if (_bomb._proximityEnabled)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_bomb.transform.position, _bomb._proximityDetectionRadius);
            }
        }

        public override void Enter()
        {
            base.Enter();

            _bomb.gameObject.SetActive(true);
        }


    }


    public class BombExplodingState : State<EProjectileState>
    {
        readonly BombBehaviour _bomb;
        public BombExplodingState(BombBehaviour bomb) : base(EProjectileState.HIT) => _bomb = bomb;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_bomb.transform.position, _bomb._explosionRadius);
        }


    }

    public class BombDestroyedState : State<EProjectileState>
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