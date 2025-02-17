using System.Collections.Generic;
using System.Linq;
using Systems.SingletonPattern;
using Unity.Netcode;
using UnityEngine;
using Utils.Extensions;

public class SpawnManager : NetworkSingleton<SpawnManager>
{
    Dictionary<ulong, GameObject> _clientServerHashMappings = new();

    public void SpawnProjectile(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Vector3 launchVelocity, List<IAbilityData> infoList)
    {
        SpawnProjectile_ServerRpc(prefab.name, position, rotation, scale, launchVelocity, infoList);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SpawnProjectile_ServerRpc(string prefabId, Vector3 position, Quaternion rotation, Vector3 scale, Vector3 launchVelocity, List<IAbilityData> infoList)
    {
        GameObject prefab = null;
        foreach (var list in NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists)
        {
            prefab = list.PrefabList.FirstOrDefault(x => x.Prefab.name == prefabId)?.Prefab;
            if (prefab != null) break;
        }
        if (prefab == null) return;

        Projectile projectile = ExtensionMethods.InstantiateAndGet<Projectile>(prefab, position, rotation, scale);//, transform, true
        GameObject instance = projectile.gameObject;
        NetworkObject networkObject = instance.GetComponent<NetworkObject>();
        networkObject.Spawn(true);

        foreach (var item in infoList)
        {
            projectile.InfoContainer.AddInfo(item);
        }

        Rigidbody rb = instance.GetComponent<Rigidbody>();
        rb.AddForce(launchVelocity * rb.mass, ForceMode.Impulse);

        _clientServerHashMappings.Add(networkObject.NetworkObjectId, instance);
    }

    public void Despawn(GameObject go)
    {
        ulong id = go.GetComponent<NetworkObject>().NetworkObjectId;
        Despawn_ServerRpc(id);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void Despawn_ServerRpc(ulong clientObjectHash)
    {
        if (_clientServerHashMappings.TryGetValue(clientObjectHash, out GameObject serverObject))
        {
            Destroy(serverObject);
            _clientServerHashMappings.Remove(clientObjectHash);
        }
    }

}