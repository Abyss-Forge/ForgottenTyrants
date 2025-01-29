using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Systems.EventBus;
using Systems.ServiceLocator;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(DamageableBehaviour), typeof(BuffableBehaviour))]
public class BodyPartDamager : MonoBehaviour
{
    DamageableBehaviour _damageable;
    BuffableBehaviour _buffable;

    [Serializable]
    private struct BodyPartData
    {
        public EBodySection BodySection;
        public BodyPart[] BodyPart;
        public float DamageMultiplier;
    }

    [SerializeField] private BodyPartData[] _bodyPartsData;

    private enum EBodySection
    {
        HEAD, TORSO, ARMS, LEGS
    }

    private Dictionary<EBodySection, (BodyPart[], float)> _bodyPartsDictionary = new();
    private List<AbilityInfoTest> _alreadyApliedInfos = new();    //TODO: dynamyc empty

    void Awake()
    {
        ServiceLocator.Global.Get(out PlayerInfo player);
        _damageable = GetComponent<DamageableBehaviour>();
        _buffable = GetComponent<BuffableBehaviour>();
        _damageable.Initialize((int)player.Stats.Health);
        _buffable.Initialize(player.Stats);

        foreach (BodyPartData data in _bodyPartsData)
        {
            _bodyPartsDictionary[data.BodySection] = (data.BodyPart, data.DamageMultiplier);
        }
    }

    void OnEnable()
    {
        _damageable.OnDeath += HandleDeath;

        foreach (BodyPartData data in _bodyPartsData)
        {
            foreach (BodyPart bodyPart in data.BodyPart)
            {
                bodyPart.OnCollision += HandleCollision;
            }
        }
    }

    void OnDisable()
    {
        _damageable.OnDeath -= HandleDeath;

        foreach (BodyPartData data in _bodyPartsData)
        {
            foreach (BodyPart bodyPart in data.BodyPart)
            {
                bodyPart.OnCollision -= HandleCollision;
            }
        }
    }

    private void HandleDeath()
    {
        EventBus<PlayerDeathEvent>.Raise(new PlayerDeathEvent());

        foreach (var entry in _bodyPartsDictionary) //Ragdoll
        {
            foreach (BodyPart bodyPart in entry.Value.Item1)
            {
                bodyPart.Rigidbody.isKinematic = false;
            }
        }
    }

    private void HandleCollision(Collision other)
    {
        Debug.Log("Hola");
        if (!other.gameObject.TryGetComponent<NetworkObject>(out NetworkObject networkObject)) return;
        Debug.Log("0");
        if (!networkObject.TryGetComponent<InfoContainer>(out InfoContainer infoContainer)) return;

        ClientData data = HostManager.Instance.GetMyClientData();
        Debug.Log("1");
        foreach (var info in infoContainer.InfoList.Where(x => x.CanApply(data) && !_alreadyApliedInfos.Contains(x)))
        {
            _alreadyApliedInfos.Add(info);
            Debug.Log("2");
            EBodySection? bodyPart = GetBodyPartHit(other.contacts);
            if (bodyPart.HasValue) ApplyDamage(bodyPart.Value, info.DamageAmount);
            /*if (info is DamageInfo damageInfo)
            {
                Debug.Log("3");
                EBodySection? bodyPart = GetBodyPartHit(other.contacts);
                if (bodyPart.HasValue) ApplyDamage(bodyPart.Value, damageInfo.DamageAmount);
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

    private EBodySection? GetBodyPartHit(ContactPoint[] contacts)
    {
        foreach (ContactPoint contact in contacts)
        {
            foreach (var entry in _bodyPartsDictionary)
            {
                foreach (BodyPart bodyPart in entry.Value.Item1)
                {
                    if (bodyPart.Collider.bounds.Contains(contact.point))
                    {
                        return entry.Key;
                    }
                }
            }
        }

        return null;
    }

    private void ApplyDamage(EBodySection bodyPart, float damage)
    {
        if (_bodyPartsDictionary.TryGetValue(bodyPart, out var bodyPartData))
        {
            damage *= bodyPartData.Item2;
            _damageable.Damage((int)damage);
            Debug.Log("Recibiste " + damage + " de daño en " + bodyPart);
        }
    }

}