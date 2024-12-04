using System;
using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;

[RequireComponent(typeof(HealthBehaviour))]
public class BodyPartDamager : MonoBehaviour
{
    HealthBehaviour _health;

    [System.Serializable]
    private struct BodyPartData
    {
        public EBodyPart BodyPart;
        public Collider[] Colliders;
        public float DamageMultiplier;
    }

    [SerializeField] private BodyPartData[] _bodyPartsData;

    private Dictionary<EBodyPart, (Collider[], float)> _bodyPartsDictionary;
    private DamageInfo _damageInfo;

    private enum EBodyPart
    {
        HEAD, TORSO, ARMS, LEGS
    }

    void Awake()
    {
        _health = GetComponentInParent<HealthBehaviour>();

        _bodyPartsDictionary = new();
        foreach (BodyPartData data in _bodyPartsData)
        {
            _bodyPartsDictionary[data.BodyPart] = (data.Colliders, data.DamageMultiplier);
        }
    }

    private void OnEnable()
    {
        _health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        _health.OnDeath -= HandleDeath;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(Tag.Enemy)) // Solo afecta proyectiles enemigos
        {
            DamageInfo dmg = other.gameObject.GetComponent<DamageInfo>();
            if (dmg != null && dmg != _damageInfo)
            {
                Debug.Log("Te ha dado: " + other.gameObject.name);
                _damageInfo = dmg;
                EBodyPart? bodyPart = GetBodyPartHit(other.contacts);

                if (bodyPart.HasValue)
                {
                    ApplyDamage(bodyPart.Value, dmg.Damage);
                }

                _damageInfo = null;
            }
        }
    }

    public void HandleDeath()
    {

        foreach (var entry in _bodyPartsDictionary)
        {
            foreach (Collider collider in entry.Value.Item1)
            {
                collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    private EBodyPart? GetBodyPartHit(ContactPoint[] contacts)
    {
        foreach (ContactPoint contact in contacts)
        {
            foreach (var entry in _bodyPartsDictionary)
            {
                foreach (Collider collider in entry.Value.Item1)
                {
                    if (collider.bounds.Contains(contact.point))
                    {
                        return entry.Key;
                    }
                }
            }
        }

        return null;
    }

    private void ApplyDamage(EBodyPart bodyPart, float baseDamage)
    {
        if (_bodyPartsDictionary.TryGetValue(bodyPart, out var bodyPartData))
        {
            baseDamage *= bodyPartData.Item2;
            _health.Damage((int)baseDamage);
            Debug.Log("Recibiste " + baseDamage + " de daño en " + bodyPart);
        }
    }

}