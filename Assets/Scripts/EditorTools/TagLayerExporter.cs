#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

public class LayerExporter : MonoBehaviour
{
    private static string folder = "Scripts/Constants";

    [MenuItem("Tools/Export/Layers", priority = 1)]
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
            string layerName = layers[i];
            if (!string.IsNullOrEmpty(layerName))
            {
                fileContent += $"   public const string {layerName.Replace(" ", "")} = \"{layerName}\";\n";
            }
        }
        fileContent += "}";

        string path = Path.Combine(Application.dataPath, folder, "SLayers.cs");
        File.WriteAllText(path, fileContent);
        Debug.Log($"Layers successfully exported to class: {path}");
    }

    [MenuItem("Tools/Export/Tags", priority = 2)]
    public static void ExportTags()
    {
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

        string fileContent = "public struct STags\n{\n";
        for (int i = 0; i < tags.Length; i++)
        {
            string tagName = tags[i];
            if (!string.IsNullOrEmpty(tagName))
            {
                fileContent += $"   public const string {tagName.Replace(" ", "")} = \"{tagName}\";\n";
            }
        }
        fileContent += "}";

        string path = Path.Combine(Application.dataPath, folder, "STags.cs");
        File.WriteAllText(path, fileContent);
        Debug.Log($"Tags successfully exported to class: {path}");
    }

    public static string GetSceneNameFromIndex(int index)
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(index);
        return System.IO.Path.GetFileNameWithoutExtension(scenePath);
    }

    public static int GetSceneIndexFromName(string name)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = GetSceneNameFromIndex(i);
            if (name == sceneName)
            {
                return i;
            }
        }
        return -1;
    }

    [MenuItem("Tools/Export/Scenes", priority = 3)]
    public static void ExportScenes()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        string fileContent = "public struct SScenes\n{\n";
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = GetSceneNameFromIndex(i);
            if (!string.IsNullOrEmpty(sceneName))
            {
                fileContent += $"   public const string {sceneName.Replace(" ", "")} = \"{sceneName}\";\n";
            }
        }
        fileContent += "}";

        string path = Path.Combine(Application.dataPath, folder, "SScenes.cs");
        File.WriteAllText(path, fileContent);
        Debug.Log($"Scenes successfully exported to class: {path}");
    }

    [MenuItem("Tools/Export/Update All", priority = 4)]
    public static void UpdateAll()
    {
        ExportLayers();
        ExportTags();
        ExportScenes();
    }

}

#endif