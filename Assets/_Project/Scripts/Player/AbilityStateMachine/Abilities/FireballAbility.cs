using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FireballAbility : AbilityStateMachine, IAbilityWithProjectile
{
    #region Specific ability properties



    #endregion
    #region Interface implementation

    [SerializeField] private GameObject _projectilePrefab;
    public GameObject ProjectilePrefab => _projectilePrefab;

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
        _fsm.Add(new AbilityReadyBaseState<FireballAbility>(this, EAbilityState.READY));
        _fsm.Add(new AbilityPreviewBaseState<FireballAbility>(this, EAbilityState.PREVIEW));
        _fsm.Add(new AbilityActiveState(this, EAbilityState.ACTIVE));
        _fsm.Add(new AbilityCooldownBaseState<FireballAbility>(this, EAbilityState.COOLDOWN));
        _fsm.Add(new AbilityLockedBaseState<FireballAbility>(this, EAbilityState.LOCKED));
    }

    private class AbilityActiveState : AbilityActiveBaseState<FireballAbility>
    {
        public AbilityActiveState(FireballAbility ability, EAbilityState id) : base(ability, id)
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
            Vector3 scale = _ability.SpawnPoint.localScale;

            Transform camera = Camera.main.transform;
            Vector3 targetPoint = camera.position + camera.forward * 100f;
            Vector3 adjustedDirection = (targetPoint - position).normalized;
            Quaternion rotation = Quaternion.LookRotation(adjustedDirection);

            GameObject instance = Instantiate(_ability.ProjectilePrefab, position, rotation, _ability.transform);
            instance.transform.localScale = scale;
            instance.transform.SetParent(null);
            instance.GetComponent<NetworkObject>().Spawn();

            Rigidbody rb = instance.GetComponent<Rigidbody>();
            Vector3 launchVelocity = adjustedDirection * _ability._launchForce;
            rb.AddForce(launchVelocity * rb.mass, ForceMode.Impulse);
        }
    }
    #endregion
}