using System;
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
        [Tooltip("Only visual")]
        public string Name;

        public BodyPart[] BodyPart;
        public float DamageMultiplier;
    }

    [SerializeField] private BodyPartData[] _bodyPartsData;

    private List<AbilityInfoTest> _alreadyApliedInfos = new();    //TODO: dynamyc empty
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
                bodyPart.OnCollisionEnterEvent += (collision) => HandleCollision(collision, bodyPart);
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
                bodyPart.OnCollisionEnterEvent -= (collision) => HandleCollision(collision, bodyPart);
            }
        }
    }

    private void HandleCollision(Collision other, BodyPart bodyPartHit)
    {
        Debug.Log("Hola");
        if (!other.gameObject.TryGetComponentInParent<NetworkObject>(out NetworkObject networkObject)) return;
        Debug.Log("0");
        if (!networkObject.TryGetComponent<InfoContainer>(out InfoContainer infoContainer)) return;

        ServiceLocator.Global.Get(out PlayerInfo player);
        Debug.Log("1");
        foreach (var info in infoContainer.InfoList.Where(x => x.CanApply(player.ClientData) && !_alreadyApliedInfos.Contains(x)))
        {
            _alreadyApliedInfos.Add(info);

            Debug.Log("2");
            BodyPartData data = _bodyPartsData.First(x => x.BodyPart.Contains(bodyPartHit));
            float damage = info.DamageAmount * infoContainer.Multiplier * data.DamageMultiplier;
            _damageable.Damage((int)damage);
            Debug.Log("Recibiste " + damage + " de daño en " + data.Name);

            /*if (info is DamageInfo damageInfo)
            {
                Debug.Log("3");
                BodyPartData data = _bodyPartsData.First(x => x.BodyPart.Contains(bodyPartHit));
                float damage = damageInfo.DamageAmount * infoContainer.Multiplier * data.DamageMultiplier;
                _damageable.Damage((int)damage);
            }
            else if (info is HealInfo healInfo)
            {
                _damageable.Heal((int)healInfo.HealAmount);
            }
            else if (info is BuffInfo buffInfo)
            {
                _buffable.ApplyBuffFromInfo(buffInfo);
            }*/
        }
    }

    private void HandleDeath()
    {
        EventBus<PlayerDeathEvent>.Raise(new PlayerDeathEvent());

        SetRagdollActive(true);
    }

    void HandleRespawn()
    {
        ServiceLocator.Global.Get(out PlayerInfo player);
        _damageable.Initialize((int)player.Stats.Health);
        _buffable.Initialize(player.Stats);

        SetRagdollActive(false);
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

}