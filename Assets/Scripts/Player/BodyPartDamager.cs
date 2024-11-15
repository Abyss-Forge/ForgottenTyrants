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

        _bodyPartsDictionary = new Dictionary<EBodyPart, (Collider[], float)>();
        foreach (BodyPartData data in _bodyPartsData)
        {
            _bodyPartsDictionary[data.BodyPart] = (data.Colliders, data.DamageMultiplier);
        }
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

/*using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;

[RequireComponent(typeof(HealthBehaviour))]
public class HitBox : MonoBehaviour
{
    HealthBehaviour _health;

    [SerializeField] private Collider[] _headColliders, _torsoColliders, _armColliders, _legColliders;
    [SerializeField] private float _headDamageMultiplier, _torsoDamageMultiplier, _armDamageMultiplier, _legDamageMultiplier;

    private enum EBodyPart
    {
        HEAD, TORSO, ARMS, LEGS
    }

    private DamageInfo _damageInfo;

    private void Awake()
    {
        _health = GetComponentInParent<HealthBehaviour>();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Te ha dado: " + other.gameObject.name);
        if (other.gameObject.CompareTag(Tag.Enemy)) // Solo afecta proyectiles enemigos
        {
            DamageInfo dmg = other.gameObject.GetComponent<DamageInfo>();
            if (dmg != null && dmg != _damageInfo)
            {
                _damageInfo = dmg;
                EBodyParts? bodyPart = null;

                foreach (ContactPoint contact in other.contacts)
                {
                    foreach (Collider collider in _headColliders)
                    {
                        if (collider.bounds.Contains(contact.point))
                        {
                            bodyPart = EBodyParts.Head;
                            break;
                        }
                    }

                    foreach (Collider collider in _torsoColliders)
                    {
                        if (collider.bounds.Contains(contact.point))
                        {
                            bodyPart = EBodyParts.Torso;
                            break;
                        }
                    }

                    foreach (Collider collider in _armColliders)
                    {
                        if (collider.bounds.Contains(contact.point))
                        {
                            bodyPart = EBodyParts.Arms;
                            break;
                        }
                    }

                    foreach (Collider collider in _legColliders)
                    {
                        if (collider.bounds.Contains(contact.point))
                        {
                            bodyPart = EBodyParts.Legs;
                            break;
                        }
                    }
                }

                if (bodyPart.HasValue)
                {
                    ApplyDamage(bodyPart.Value, dmg.Damage);
                }

                _damageInfo = null;
            }
        }
    }

    private void ApplyDamage(EBodyParts bodyPart, float baseDamage)
    {
        switch (bodyPart)
        {
            case EBodyParts.Head:
                baseDamage *= _headDamageMultiplier;
                break;
            case EBodyParts.Torso:
                baseDamage *= _torsoDamageMultiplier;
                break;
            case EBodyParts.Arms:
                baseDamage *= _armDamageMultiplier;
                break;
            case EBodyParts.Legs:
                baseDamage *= _legDamageMultiplier;
                break;
        }

        _health.Damage((int)baseDamage);

        Debug.Log("Recibió " + baseDamage + " de daño en " + bodyPart);
    }

}
*/
