using Systems.ServiceLocator;
using UnityEngine;

public class BombAbility : AbilityStateMachine, IAbilityWithProjectile
{
    #region Specific ability properties



    #endregion
    #region Interface implementation

    [SerializeField] private Projectile _projectilePrefab;
    public GameObject ProjectilePrefab => _projectilePrefab.gameObject;

    [SerializeField] private int _projectileAmount = 1;
    public int ProjectileAmount => _projectileAmount;

    [SerializeField] private float _projectileThreshold = 0f;
    public float ProjectileThreshold => _projectileThreshold;

    [SerializeField] private float _launchForce = 10f;
    public float LaunchForce => _launchForce;

    #endregion
    #region States

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyBaseState<BombAbility>(this, EAbilityState.READY));
        _fsm.Add(new AbilityPreviewBaseState<BombAbility>(this, EAbilityState.PREVIEW));
        _fsm.Add(new AbilityActiveState(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<BombAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<BombAbility>(this, EAbilityState.LOCKED));
    }

    private class AbilityActiveState : AbilityActiveBaseState<BombAbility>
    {
        public AbilityActiveState(BombAbility ability, EAbilityState id) : base(ability, id)
        {
        }

        private float _timer;
        private int _cycles;

        public override void Enter()
        {
            _timer = 0;
            _cycles = 0;
        }

        public override void Update()
        {
            PerformBurst();
        }

        private void PerformBurst()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _ability.SpawnProjectile();
                _timer = _ability.ProjectileThreshold;
                _cycles++;
                if (_cycles >= _ability.ProjectileAmount)
                {
                    _ability._fsm.TransitionTo(EAbilityState.COOLDOWN);
                }
            }
        }
    }
    #endregion

    private void SpawnProjectile()
    {
        Vector3 position = SpawnPoint.position;
        Quaternion rotation = SpawnPoint.rotation;
        Vector3 scale = SpawnPoint.localScale;

        Transform camera = Camera.main.transform;
        Vector3 targetPoint = camera.position + camera.forward * 100f;
        Vector3 adjustedDirection = (targetPoint - position).normalized;
        adjustedDirection.y += 0.5f;

        ServiceLocator.Global.Get(out PlayerController player);
        Vector3 playerVelocity = player.Velocity;
        playerVelocity.y = 0;
        Vector3 launchVelocity = adjustedDirection * _launchForce + playerVelocity;

        SpawnManager.Instance.SpawnProjectile(_projectilePrefab.gameObject, position, rotation, scale, launchVelocity, _infoList);
    }
}