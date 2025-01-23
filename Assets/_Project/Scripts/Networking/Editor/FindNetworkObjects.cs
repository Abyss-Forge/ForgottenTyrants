using UnityEditor;
using UnityEngine;
using Unity.Netcode;

public class FindNetworkObjects : EditorWindow
{
    [MenuItem("Tools/Find NetworkObject Prefabs")]
    public static void FindPrefabsWithNetworkObject()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int count = 0;

        foreach (string guid in prefabGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                NetworkObject networkObject = prefab.GetComponent<NetworkObject>();

                if (networkObject != null)
                {
                    ulong globalObjectIdHash = networkObject.PrefabIdHash;
                    Debug.Log($"Hash : {globalObjectIdHash} | Path: {assetPath}", prefab);
                    count++;
                }
            }
        }

        Debug.Log($"Se encontraron {count} prefabs con un componente NetworkObject.");
    }
}