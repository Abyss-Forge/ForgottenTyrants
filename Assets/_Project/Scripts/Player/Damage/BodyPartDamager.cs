using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForgottenTyrants;
using Systems.EventBus;
using UnityEngine;

[RequireComponent(typeof(DamageableBehaviour))]
public class BodyPartDamager : MonoBehaviour
{
    DamageableBehaviour _damageable;

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
    private List<AbilityInfo> _alreadyApliedInfos = new();    //TODO: dynamyc empty

    void Awake()
    {
        _damageable = GetComponentInParent<DamageableBehaviour>();

        foreach (BodyPartData data in _bodyPartsData)
        {
            _bodyPartsDictionary[data.BodySection] = (data.BodyPart, data.DamageMultiplier);
        }
    }

    private void OnEnable()
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

    private void OnDisable()
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
        ClientData data = HostManager.Instance.GetMyClientData();

        InfoContainer container = other.gameObject.GetComponent<InfoContainer>();
        if (container == null) return;
        Debug.Log("Impacto" + container.InfoList.OfType<DamageInfo>().Count());
        foreach (var info in container.InfoList.OfType<DamageInfo>())   //TODO heal y buffs
        {
            Debug.Log("Impactrueno");
            if (info.CanApply(data) && !_alreadyApliedInfos.Contains(info))
            {
                Debug.Log("Te ha dado: " + other.gameObject.name);
                _alreadyApliedInfos.Add(info);

                EBodySection? bodyPart = GetBodyPartHit(other.contacts);
                if (bodyPart.HasValue) ApplyDamage(bodyPart.Value, info.DamageAmount);
            }
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

    private void ApplyDamage(EBodySection bodyPart, float baseDamage)
    {
        if (_bodyPartsDictionary.TryGetValue(bodyPart, out var bodyPartData))
        {
            baseDamage *= bodyPartData.Item2;
            _damageable.Damage((int)baseDamage);
            Debug.Log("Recibiste " + baseDamage + " de daño en " + bodyPart);
        }
    }

}