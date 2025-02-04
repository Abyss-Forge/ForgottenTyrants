using System.Collections;
using System.Collections.Generic;
using Systems.ServiceLocator;
using Unity.Netcode;
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
                _ability.SpawnProjectileServerRpc();
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

    [ServerRpc(RequireOwnership = false)]
    private void SpawnProjectileServerRpc()
    {
        Vector3 position = SpawnPoint.position;
        Quaternion rotation = SpawnPoint.rotation;
        Vector3 scale = SpawnPoint.localScale;

        Projectile projectile = Instantiate(_projectilePrefab, position, rotation, transform);
        foreach (var item in _infoList)
        {
            projectile.InfoContainer.Add(item);
        }
        _infoList.Clear();

        GameObject instance = projectile.gameObject;
        instance.transform.localScale = scale;
        instance.transform.SetParent(null);
        instance.GetComponent<NetworkObject>().Spawn(true);

        Transform camera = Camera.main.transform;
        Vector3 targetPoint = camera.position + camera.forward * 100f;
        Vector3 adjustedDirection = (targetPoint - position).normalized;
        adjustedDirection.y += 0.5f;

        ServiceLocator.Global.Get(out PlayerController player);
        Vector3 playerVelocity = player.Velocity;
        playerVelocity.y = 0;
        Vector3 launchVelocity = adjustedDirection * _launchForce + playerVelocity;

        Rigidbody rb = instance.GetComponent<Rigidbody>();
        rb.AddForce(launchVelocity * rb.mass, ForceMode.Impulse);
    }
}