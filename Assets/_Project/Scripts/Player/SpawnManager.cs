using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils.Extensions;

public class SpawnManager : NetworkSingleton<SpawnManager>
{
    public void SpawnProjectile(string prefabAddress, Vector3 position, Quaternion rotation, Vector3 scale, Vector3 launchVelocity, List<AbilityInfoTest> infoList)
    {
        SpawnProjectile_ServerRpc(prefabAddress, position, rotation, scale, launchVelocity, infoList);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void SpawnProjectile_ServerRpc(string prefabAddress, Vector3 position, Quaternion rotation, Vector3 scale, Vector3 launchVelocity, List<AbilityInfoTest> infoList)
    {
        // Usamos Addressables para cargar el prefab
        LoadPrefabAsync(prefabAddress, position, rotation, scale, launchVelocity, infoList);
    }

    private void LoadPrefabAsync(string prefabAddress, Vector3 position, Quaternion rotation, Vector3 scale, Vector3 launchVelocity, List<AbilityInfoTest> infoList)
    {
        // Cargamos el prefab de forma asíncrona usando su "Address"
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(prefabAddress);

        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = op.Result;
                Debug.Log(prefab.name);

                // Instanciamos el proyectil
                Projectile projectile = ExtensionMethods.InstantiateAndGet<Projectile>(prefab, position, rotation, scale, transform, true);
                projectile.gameObject.GetComponent<NetworkObject>().Spawn(true);

                foreach (var item in infoList)
                {
                    projectile.InfoContainer.Add(item);
                }

                Rigidbody rb = projectile.gameObject.GetComponent<Rigidbody>();
                rb.AddForce(launchVelocity * rb.mass, ForceMode.Impulse);
            }
            else
            {
                Debug.LogError("No se pudo cargar el prefab desde Addressables.");
            }
        };
    }
}
