using System.Collections.Generic;
using Systems.ServiceLocator;
using UnityEngine;

public class ServiceDatabase : MonoBehaviour
{
    [SerializeField] List<Object> _services;

    void Awake()
    {
        ServiceLocator sl = ServiceLocator.Global;
        foreach (Object service in _services)
        {
            sl.Register(service.GetType(), service);
        }
    }

}