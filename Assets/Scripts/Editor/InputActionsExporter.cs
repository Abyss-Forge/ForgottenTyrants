#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class InputActionsExporter : MonoBehaviour
{

    [MenuItem("CONTEXT/InputActionAsset/Export Input Actions", priority = 1)]
    public static void ExportInputActions(MenuCommand command)
    {
        InputActionAsset inputActionAsset = command.context as InputActionAsset;

        if (inputActionAsset == null)
        {
            Debug.LogError("No Input Action Asset found.");
            return;
        }

        string fileContent = "public enum InputActions\n{\n";

        // Recorre los mapas de acción y sus acciones
        foreach (var map in inputActionAsset.actionMaps)
        {
            foreach (var action in map.actions)
            {
                // Asegúrate de que el nombre de la acción sea un identificador válido para el enum
                string actionName = action.name.Replace(" ", "").Replace("-", "_");
                fileContent += $"   {actionName},\n";
            }
        }

        fileContent += "}\n";

        // Define la ruta del archivo a exportar
        string path = Path.Combine(Application.dataPath, "InputActions.cs");

        File.WriteAllText(path, fileContent);
        AssetDatabase.Refresh(); // Refresca el AssetDatabase para que reconozca el nuevo archivo
        Debug.Log($"Input actions successfully exported to: {path}");
    }
}

#endif
