using System;
using System.Threading.Tasks;
using ForgottenTyrants;
using Systems.FSM;
using Unity.Netcode;
using UnityEngine;
using Utils.Extensions;

[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(AbilityDataContainer))]
public abstract class Projectile : NetworkBehaviour
{
    protected Rigidbody _rigidbody { get; set; }
    protected Collider _collider { get; set; }
    public AbilityDataContainer InfoContainer { get; protected set; }

    public enum EProjectileState
    {
        LIVE, IMPACTING, DESTROYED
    }

    [SerializeField] protected Transform _modelRoot;
    [SerializeField] protected ParticleSystem _vfx;
    [SerializeField] protected AudioSource _sfx;

    [SerializeField] protected float _lifetime = 5, _gravityMultiplier = 1;

    [Header("Ricochet")]
    [SerializeField] protected bool _hasRicochet;
    [SerializeField] protected int _ricochets = 5;
    [SerializeField, Range(0, 90)] protected float _maxRicochetAngle = 45;

    protected float _lifetimeTimer;
    protected int _remainingRicochets;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        InfoContainer = GetComponent<AbilityDataContainer>();

        _lifetimeTimer = _lifetime;
        _remainingRicochets = _ricochets;

        _fsm = new();
        InitializeStates();
        _fsm.TransitionTo(EProjectileState.LIVE);
    }

    void OnCollisionEnter(Collision other)
    {
        if (_fsm.CurrentState is ProjectileLiveState)
        {
            (_fsm.CurrentState as ProjectileLiveState)?.OnCollide(other);
        }
    }

    protected virtual void OnCollide(Collision other)
    {
        if (!IsDirectHit(other.gameObject) && _hasRicochet && _remainingRicochets > 0)
        {
            Vector3 collisionNormal = other.contacts[0].normal;
            Vector3 incomingDirection = _rigidbody.velocity.normalized;
            float angle = Vector3.Angle(incomingDirection, -collisionNormal) - 90;

            if (angle <= _maxRicochetAngle)
            {
                _remainingRicochets--;
                return;
                // Si hubiese que rebotar a mano:
                //  Vector3 reflectedDirection = Vector3.Reflect(incomingDirection, collisionNormal);
                //  _rigidbody.velocity = reflectedDirection * _rigidbody.velocity.magnitude;
            }
        }
        _fsm.TransitionTo(EProjectileState.IMPACTING);
    }

    protected virtual bool IsDirectHit(GameObject go)
    {
        return go.layer == Layer.Player;
    }

    protected virtual async Task OnHit()    //TODO vfx & sfx on impact, ref to ExplosiveProjectile
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        _modelRoot.gameObject.SetActive(false);

        _fsm.TransitionTo(EProjectileState.DESTROYED);
    }

    private void ApplyGravity()
    {
        if (_rigidbody.useGravity) _rigidbody.AddForce(Physics.gravity * (_gravityMultiplier - 1f), ForceMode.Acceleration);
    }    // Al multiplicador se le resta 1 porque  por defecto la gravedad ya se aplica una vez al tener rigidbodys

    private void CalculateLifetime()
    {
        if (_lifetimeTimer > 0)
        {
            _lifetimeTimer -= Time.deltaTime;
        }
        else
        {
            OnLifetimeEnd();
        }
    }

    protected virtual void OnLifetimeEnd()
    {
        _fsm.TransitionTo(EProjectileState.IMPACTING);
    }

    #region State Machine

    public FiniteStateMachine<EProjectileState> _fsm { get; private set; }

    void Update() => _fsm.Update();
    void FixedUpdate() => _fsm.FixedUpdate();
    void LateUpdate() => _fsm.LateUpdate();

    protected virtual void InitializeStates()
    {
        _fsm.Add(new ProjectileLiveState(this));
        _fsm.Add(new ProjectileImpactingState(this));
        _fsm.Add(new ProjectileDestroyedState(this));
    }

    protected class ProjectileLiveState : State<EProjectileState>
    {
        readonly Projectile _projectile;
        public ProjectileLiveState(Projectile projectile) : base(EProjectileState.LIVE) => _projectile = projectile;

        public override void Enter()
        {
            base.Enter();

            _projectile.gameObject.SetActive(true);
        }

        public override void Update()
        {
            base.Update();

            _projectile.CalculateLifetime();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _projectile.ApplyGravity();
        }

        public void OnCollide(Collision other)
        {
            _projectile.OnCollide(other);
        }
    }

    protected class ProjectileImpactingState : State<EProjectileState>
    {
        readonly Projectile _projectile;
        public ProjectileImpactingState(Projectile projectile) : base(EProjectileState.IMPACTING) => _projectile = projectile;

        public override async void Enter()
        {
            base.Enter();

            await _projectile.OnHit();
        }
    }

    protected class ProjectileDestroyedState : State<EProjectileState>
    {
        readonly Projectile _projectile;
        public ProjectileDestroyedState(Projectile projectile) : base(EProjectileState.DESTROYED) => _projectile = projectile;

        public override void Enter()
        {
            base.Enter();

            SpawnManager.Instance.Despawn(_projectile.gameObject);
        }
    }

    #endregion
}