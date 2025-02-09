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

    private enum EBodySection
    {
        HEAD, TORSO, ARMS, LEGS
    }

    [Serializable]
    private struct BodyPartData
    {
        [SerializeField, Tooltip("Only visual")] string _name;
        public EBodySection BodySection;

        public BodyPart[] BodyPart;
        public float DamageMultiplier;
    }

    [SerializeField] private BodyPartData[] _bodyPartsData;

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
                bodyPart.OnCollisionEnterEvent += (collision) => HandleCollision(collision, bodyPart);

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
                bodyPart.OnCollisionEnterEvent -= (collision) => HandleCollision(collision, bodyPart);
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

    private void HandleCollision(Collision other, BodyPart bodyPartHit)
    {
        Debug.Log("Hola");
        if (!other.gameObject.TryGetComponent<NetworkObject>(out NetworkObject networkObject)) return;
        Debug.Log("0");
        if (!networkObject.TryGetComponent<InfoContainer>(out InfoContainer infoContainer)) return;

        ServiceLocator.Global.Get(out PlayerInfo player);
        Debug.Log("1");
        foreach (var info in infoContainer.InfoList.Where(x => x.CanApply(player.ClientData) && !_alreadyApliedInfos.Contains(x)))
        {
            _alreadyApliedInfos.Add(info);

            Debug.Log("2");
            BodyPartData data = FindBodyPartData(bodyPartHit);
            float damage = info.DamageAmount * infoContainer.Multiplier * data.DamageMultiplier;
            _damageable.Damage((int)damage);
            Debug.Log("Recibiste " + damage + " de daño en " + data.BodySection);

            /*if (info is DamageInfo damageInfo)
            {
                Debug.Log("3");
                BodyPartData data = FindBodyPartSection(bodyPartHit);
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

    private BodyPartData FindBodyPartData(BodyPart hitBodyPart)
    {
        return _bodyPartsData.First(x => x.BodyPart.Contains(hitBodyPart));
    }

}