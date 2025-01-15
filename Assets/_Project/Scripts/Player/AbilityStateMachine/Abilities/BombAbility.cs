using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BombAbility : AbilityStateMachine, IAbilityWithProjectile
{
    #region Specific ability properties

    [SerializeField] private float _force;

    #endregion
    #region Interface implementation

    [SerializeField] private GameObject _projectilePrefab;
    public GameObject ProjectilePrefab => _projectilePrefab;

    [SerializeField] private int _projectileAmount = 1;
    public int ProjectileAmount => _projectileAmount;

    [SerializeField] private float _projectileThreshold = 0f;
    public float ProjectileThreshold => _projectileThreshold;

    #endregion
    #region States

    protected override void InitializeStates()
    {
        _fsm.Add(new AbilityReadyBaseState<BombAbility>(this, EAbilityState.READY));
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
                SpawnBomb();
                _timer = _ability.ProjectileThreshold;
                _cycles++;
                if (_cycles >= _ability.ProjectileAmount)
                {
                    _ability._fsm.TransitionTo(EAbilityState.COOLDOWN);
                }
            }
        }

        private void SpawnBomb()
        {
            Vector3 position = _ability.SpawnPoint.position;
            Quaternion rotation = _ability.SpawnPoint.rotation;

            GameObject instance = Instantiate(_ability.ProjectilePrefab, position, rotation, _ability.transform);
            instance.transform.SetParent(null); // esto es para que spawnee en la misma escena si hay aditivas
            instance.GetComponent<NetworkObject>().Spawn();

            Transform camera = Camera.main.transform;
            Vector3 targetPoint = camera.position + camera.forward * 100f;
            Vector3 adjustedDirection = (targetPoint - position).normalized;
            adjustedDirection.y += 0.5f;

            Rigidbody rb = instance.GetComponent<Rigidbody>();
            rb.AddForce(adjustedDirection * _ability._force * rb.mass, ForceMode.Impulse);
        }

    }
    #endregion
}