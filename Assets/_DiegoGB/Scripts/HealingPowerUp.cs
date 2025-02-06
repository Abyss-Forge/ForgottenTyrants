using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealingPowerUp : NetworkBehaviour
{
    [SerializeField] private int _healAmount;
    [SerializeField] GameObject _test;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.gameObject.CompareTag("Player"))
        {
            PlayVisualEffectClientRpc();
            Debug.Log($"{other.gameObject} ha sido curado {_healAmount}");
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void PlayVisualEffectClientRpc()
    {
        _test.GetComponentInChildren<ParticleSystem>().Play();
    }
}
