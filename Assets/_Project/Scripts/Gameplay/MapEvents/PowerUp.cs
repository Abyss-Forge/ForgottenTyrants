using ForgottenTyrants;
using Unity.Netcode;
using UnityEngine;
using Utils.Extensions;

public abstract class PowerUp : NetworkBehaviour
{
    protected AbilityDataContainer _container;

    protected abstract void CalculateData();

    void Awake()
    {
        _container = GetComponentInParent<AbilityDataContainer>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        CalculateData();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.gameObject.CompareLayer(Layer.Player))
        {
            Debug.Log($"Power up recogido");
            Despawn();
        }
    }

    protected void Despawn()
    {
        GetComponentInParent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

}