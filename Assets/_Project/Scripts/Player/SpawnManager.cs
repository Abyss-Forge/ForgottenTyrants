using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Utils.Extensions;

public class SpawnManager : NetworkSingleton<SpawnManager>
{

    public void SpawnProjectile(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Vector3 launchVelocity, List<AbilityInfoTest> infoList)
    {
        SpawnProjectile_ServerRpc(prefab.name, position, rotation, scale, launchVelocity, infoList);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SpawnProjectile_ServerRpc(string prefabId, Vector3 position, Quaternion rotation, Vector3 scale, Vector3 launchVelocity, List<AbilityInfoTest> infoList)
    {
        GameObject prefab = null;
        foreach (var list in NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists)
        {
            prefab = list.PrefabList.FirstOrDefault(x => x.Prefab.name == prefabId)?.Prefab;
            if (prefab != null) break;
        }
        if (prefab == null) return;

        Projectile projectile = ExtensionMethods.InstantiateAndGet<Projectile>(prefab, position, rotation, scale, transform, true);
        projectile.gameObject.GetComponent<NetworkObject>().Spawn(true);

        foreach (var item in infoList)
        {
            projectile.InfoContainer.Add(item);
        }

        Rigidbody rb = projectile.gameObject.GetComponent<Rigidbody>();
        rb.AddForce(launchVelocity * rb.mass, ForceMode.Impulse);
    }

}