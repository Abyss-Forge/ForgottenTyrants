using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealingPowerUp : NetworkBehaviour
{
    [SerializeField] private int _healAmount;
    [SerializeField] GameObject _healingPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.gameObject.CompareTag("Player"))
        {
            // Ejecuta el efecto visual en todos los clientes y en el host
            PlayVisualEffect_ClientRpc();

            //TODO implementar healing.
            Debug.Log($"{other.gameObject} ha sido curado {_healAmount}");

            // Despawnea el objeto de red y lo destruye en la escena
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayVisualEffect_ClientRpc()
    {
        _healingPrefab.GetComponentInChildren<ParticleSystem>().Play();
    }
}
