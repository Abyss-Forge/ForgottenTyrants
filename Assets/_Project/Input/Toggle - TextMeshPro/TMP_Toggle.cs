#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;

public class TMP_Toggle : MonoBehaviour
{
    private static string _prefabName = "TMP_Toggle";

    [MenuItem("GameObject/UI/Toggle - TextMeshPro")]
    private static void InstantiateToggle(MenuCommand menuCommand)
    {
        string scriptPath = FindScriptPath(nameof(TMP_Toggle));
        if (string.IsNullOrEmpty(scriptPath)) return;

        string folderPath = Path.GetDirectoryName(scriptPath);
        string prefabPath = Path.Combine(folderPath, $"{_prefabName}.prefab");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null) return;

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = "Toggle (TMP)";

        if (instance != null)
        {
            // Si se seleccionó un objeto en la jerarquía, lo establece como padre del nuevo objeto
            GameObject parent = menuCommand.context as GameObject;
            if (parent != null)
            {
                instance.transform.SetParent(parent.transform);
            }

            // Ajusta la posición local del objeto
            instance.transform.localPosition = Vector3.zero;

            // Registra la creación del objeto para deshacer en el editor
            Undo.RegisterCreatedObjectUndo(instance, "Create " + instance.name);
            Selection.activeObject = instance;

            PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }
        else
        {
            Debug.LogError("No se pudo instanciar el prefab. Asegúrate de que el prefab esté configurado correctamente.");
        }
    }

    private static string FindScriptPath(string scriptName)
    {
        string[] guids = AssetDatabase.FindAssets($"t:Script {scriptName}");
        if (guids.Length == 0) return null;
        return AssetDatabase.GUIDToAssetPath(guids[0]);
    }

}

#endif