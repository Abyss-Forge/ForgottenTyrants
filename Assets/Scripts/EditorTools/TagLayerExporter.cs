#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;

public class LayerExporter : MonoBehaviour
{
    private static string folder = "Scripts/Constants";

    [MenuItem("Tools/Export/Layers")]
    public static void ExportLayers()
    {
        int layerCount = 32;
        string[] layers = new string[layerCount];

        for (int i = 0; i < layerCount; i++)
        {
            layers[i] = LayerMask.LayerToName(i);
        }

        string fileContent = "public struct SLayers\n{\n";
        for (int i = 0; i < layers.Length; i++)
        {
            if (!string.IsNullOrEmpty(layers[i]))
            {
                fileContent += $"   public const string {layers[i].Replace(" ", "")} = \"{layers[i]}\";\n";
            }
        }
        fileContent += "}";

        string path = Path.Combine(Application.dataPath, folder, "SLayers.cs");
        File.WriteAllText(path, fileContent);
        Debug.Log($"Layers successfully exported to class: {path}");
    }

    [MenuItem("Tools/Export/Tags")]
    public static void ExportTags()
    {
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

        string fileContent = "public struct STags\n{\n";
        for (int i = 0; i < tags.Length; i++)
        {
            if (!string.IsNullOrEmpty(tags[i]))
            {
                fileContent += $"   public const string {tags[i].Replace(" ", "")} = \"{tags[i]}\";\n";
            }
        }
        fileContent += "}";

        string path = Path.Combine(Application.dataPath, folder, "STags.cs");
        File.WriteAllText(path, fileContent);
        Debug.Log($"Tags successfully exported to class: {path}");
    }


}

#endif