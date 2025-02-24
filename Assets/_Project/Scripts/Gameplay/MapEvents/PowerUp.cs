using ForgottenTyrants;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AbilityDataContainer))]
public abstract class PowerUp : NetworkBehaviour
{
    protected AbilityDataContainer _container;

    protected abstract void CalculateData();

    void Awake()
    {
        _container = GetComponent<AbilityDataContainer>();

        CalculateData();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tag.Player))
        {
            Debug.Log($"En proceso");
            Despawn();
        }
    }

    protected void Despawn()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}