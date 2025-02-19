using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Systems.EventBus;
using Systems.ServiceLocator;
using Unity.Netcode;
using UnityEngine;
using Utils.Extensions;

[RequireComponent(typeof(DamageableBehaviour), typeof(BuffableBehaviour))]
public class BodyPartDamager : MonoBehaviour
{
    DamageableBehaviour _damageable;
    BuffableBehaviour _buffable;

    [Serializable]
    private struct BodyPartData
    {
        [Tooltip("Only visual")] public string Name;
        public BodyPart[] BodyPart;
        public float DamageMultiplier;
    }
    [SerializeField] private BodyPartData[] _bodyPartsData;

    private bool _isAlive = true, _isInvincible;
    private List<int> _alreadyAppliedHashes = new();    //TODO: dynamyc empty

    private EventBinding<PlayerRespawnEvent> _playerRespawnEventBinding;

    void Awake()
    {
        ServiceLocator.Global.Get(out PlayerInfo player);
        _damageable = GetComponent<DamageableBehaviour>();
        _buffable = GetComponent<BuffableBehaviour>();
        _damageable.Initialize((int)player.Stats.Health);
        _buffable.Initialize(player.Stats);
    }

    void OnEnable()
    {
        _playerRespawnEventBinding = new EventBinding<PlayerRespawnEvent>(HandleRespawn);
        EventBus<PlayerRespawnEvent>.Register(_playerRespawnEventBinding);

        _damageable.OnDeath += HandleDeath;

        foreach (BodyPartData data in _bodyPartsData)
        {
            foreach (BodyPart bodyPart in data.BodyPart)
            {
                bodyPart.OnCollisionEnterEvent += (collision) => HandleCollision(collision.gameObject, bodyPart);
                bodyPart.OnTriggerEnterEvent += (collision) => HandleCollision(collision.gameObject, bodyPart);
            }
        }
    }

    void OnDisable()
    {
        EventBus<PlayerRespawnEvent>.Deregister(_playerRespawnEventBinding);

        _damageable.OnDeath -= HandleDeath;

        foreach (BodyPartData data in _bodyPartsData)
        {
            foreach (BodyPart bodyPart in data.BodyPart)
            {
                bodyPart.OnCollisionEnterEvent -= (collision) => HandleCollision(collision.gameObject, bodyPart);
                bodyPart.OnTriggerEnterEvent -= (collision) => HandleCollision(collision.gameObject, bodyPart);
            }
        }
    }

    private void HandleCollision(GameObject other, BodyPart bodyPartHit)
    {
        if (!_isAlive || _isInvincible) return;
        if (!other.TryGetComponentInParent<NetworkObject>(out NetworkObject networkObject)) return;
        if (!networkObject.TryGetComponent<AbilityDataContainer>(out AbilityDataContainer container)) return;
        Debug.Log("Damage detected");

        ServiceLocator.Global.Get(out PlayerInfo player);
        foreach (var data in container.DataList.Where(x => x.AbilityData.CanApply(player.ClientData) && !_alreadyAppliedHashes.Contains(x.AbilityData.Hash)))
        {
            if (!_isAlive || _isInvincible) return;

            _alreadyAppliedHashes.Add(data.AbilityData.Hash);
            Debug.Log("Damage applied");

            if (data is DamageData damageData)
            {
                BodyPartData bodyPart = _bodyPartsData.First(x => x.BodyPart.Contains(bodyPartHit));
                float damage = damageData.DamageAmount * container.Multiplier * bodyPart.DamageMultiplier;
                _damageable.Damage((int)damage);
                Debug.Log("Damaging " + bodyPart.Name);
            }
            else if (data is HealData healData)
            {
                Debug.Log("Healing");
                _damageable.Heal((int)healData.HealAmount);
            }
            else if (data is BuffData buffData)
            {
                Debug.Log("Buffing");
                _buffable.ApplyBuffFromData(buffData);
            }
        }
    }

    private void HandleDeath()
    {
        _isAlive = false;

        EventBus<PlayerDeathEvent>.Raise(new PlayerDeathEvent());

        SetRagdollActive(true);
    }

    private void HandleRespawn(PlayerRespawnEvent @event)
    {
        ServiceLocator.Global.Get(out PlayerInfo player);
        _damageable.Initialize((int)player.Stats.Health);
        _buffable.Initialize(player.Stats);

        SetRagdollActive(false);
        StartCoroutine(ApplyInvincibility(@event.SpawnInvincibilityTime));

        _isAlive = true;
    }

    private void SetRagdollActive(bool enable = true)
    {
        foreach (BodyPartData data in _bodyPartsData)
        {
            foreach (BodyPart bodyPart in data.BodyPart)
            {
                bodyPart.Rigidbody.isKinematic = !enable;
            }
        }
    }

    private IEnumerator ApplyInvincibility(float invincibilityTime)
    {
        _isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        _isInvincible = false;
    }

}