using System.Collections;
using System.Collections.Generic;
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
                SpawnProjectile();
                _timer = _ability.ProjectileThreshold;
                _cycles++;
                if (_cycles >= _ability.ProjectileAmount)
                {
                    _ability._fsm.TransitionTo(EAbilityState.COOLDOWN);
                }
            }
        }

        private void SpawnProjectile()
        {
            Vector3 position = _ability.SpawnPoint.position;
            Quaternion rotation = _ability.SpawnPoint.rotation;
            Vector3 scale = _ability.SpawnPoint.localScale;

            Projectile projectile = Instantiate(_ability._projectilePrefab, position, rotation, _ability.transform);
            foreach (var item in _ability._infoList)
            {
                projectile.InfoContainer.Add(item);
            }
            GameObject instance = projectile.gameObject;
            instance.transform.localScale = scale;
            instance.GetComponent<NetworkObject>().Spawn();

            Transform camera = Camera.main.transform;
            Vector3 targetPoint = camera.position + camera.forward * 100f;
            Vector3 adjustedDirection = (targetPoint - position).normalized;
            adjustedDirection.y += 0.5f;

            Rigidbody rb = instance.GetComponent<Rigidbody>();
            Vector3 playerVelocity = _ability.GetComponentInParent<PlayerController>()?.Velocity ?? Vector3.zero; //TODO hacer con service locator
            playerVelocity.y = 0;
            Vector3 launchVelocity = adjustedDirection * _ability._launchForce + playerVelocity;
            rb.AddForce(launchVelocity * rb.mass, ForceMode.Impulse);

            instance.transform.SetParent(null); // esto es para que spawnee en la misma escena si hay aditivas, deberia ir arriba pero como usarmos un GetComponentInParent pues...
        }

    }
    #endregion
}