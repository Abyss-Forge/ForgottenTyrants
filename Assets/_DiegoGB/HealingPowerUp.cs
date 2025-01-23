using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealingPowerUp : NetworkBehaviour
{
    [SerializeField] private int _healAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{other.gameObject} ha sido curado {_healAmount}");
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}
