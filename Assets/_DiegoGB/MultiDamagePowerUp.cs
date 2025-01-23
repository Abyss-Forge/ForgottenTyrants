using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiDamagePowerUp : NetworkBehaviour
{
    [SerializeField] private int _damageAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"En proceso");
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}
