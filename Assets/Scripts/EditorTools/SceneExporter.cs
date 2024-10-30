#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

public class SceneExporter : MonoBehaviour
{
    private static string _folder = SGlobalSettings.ExportedConstantsFolder;

    [MenuItem("Tools/Export/Scenes", priority = 1)]
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

        string path = Path.Combine(Application.dataPath, _folder, "SScenes.cs");
        File.WriteAllText(path, fileContent);
        Debug.Log($"Scenes successfully exported to class: {path}");
    }

    public static string GetSceneNameFromIndex(int index)
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(index);
        return Path.GetFileNameWithoutExtension(scenePath);
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
}

#endif