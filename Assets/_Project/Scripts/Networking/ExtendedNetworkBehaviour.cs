using System;
using Unity.Netcode;
using UnityEngine;

public enum ENetworkProperty
{
    IS_SPAWNED,
    IS_LOCAL_PLAYER,
    IS_OWNER,
    IS_OWNED_BY_SERVER,
    SERVER_IS_HOST,
    IS_SERVER,
    IS_HOST,
    IS_CLIENT,
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class NetworkPropertyAttribute : PropertyAttribute
{
    public bool IsExclusive;

    public struct NetworkPropertyAttributeParams
    {
        public bool IsExclusive;
    }

    public NetworkPropertyAttribute(ENetworkProperty property)
    {
    }
}

public abstract class ExtendedNetworkBehaviour : NetworkBehaviour
{
    /*
    [NetworkProperty(ENetworkProperty.IS_CLIENT)]
    [NetworkProperty(ENetworkProperty.IS_OWNER)]
    protected abstract void OnNetworkSpawnAs();

    public void Test()
    {
        if (IsClient || IsOwner)
        {
            //Do something
        }
    }

    public override void OnNetworkSpawn()
    {
        OnNetworkSpawnAs();
    }
    */

    public override void OnNetworkSpawn()
    {
        if (IsOwner) OnNetworkSpawnAsOwner();
        if (IsServer) OnNetworkSpawnAsServer();
        if (IsHost) OnNetworkSpawnAsHost();
        if (IsClient) OnNetworkSpawnAsClient();
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner) OnNetworkDespawnAsOwner();
        if (IsServer) OnNetworkDespawnAsServer();
        if (IsHost) OnNetworkDespawnAsHost();
        if (IsClient) OnNetworkDespawnAsClient();
    }

    protected abstract void OnNetworkSpawnAsOwner();
    protected abstract void OnNetworkSpawnAsServer();
    protected abstract void OnNetworkSpawnAsHost();
    protected abstract void OnNetworkSpawnAsClient();

    protected abstract void OnNetworkDespawnAsOwner();
    protected abstract void OnNetworkDespawnAsServer();
    protected abstract void OnNetworkDespawnAsHost();
    protected abstract void OnNetworkDespawnAsClient();
}