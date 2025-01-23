using System.Collections.Generic;
using Systems.ServiceLocator;
using Unity.Netcode;
using UnityEngine;

public class ServiceDatabase : NetworkBehaviour
{
    [SerializeField] List<Object> _services;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        ServiceLocator sl = ServiceLocator.Global;
        foreach (Object service in _services)
        {
            sl.Register(service.GetType(), service);
        }
    }

}