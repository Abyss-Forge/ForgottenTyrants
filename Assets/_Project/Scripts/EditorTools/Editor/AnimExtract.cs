using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AnimExtract : MonoBehaviour
{
    [MenuItem("Tools/Extract Animations from FBX")]
    static void ExtractAnimations()
    {
        // Selecciona un FBX desde la ventana emergente
        string fbxPath = EditorUtility.OpenFilePanel("Select FBX", Application.dataPath, "fbx");
        if (string.IsNullOrEmpty(fbxPath)) return;

        string relativePath = "Assets" + fbxPath.Substring(Application.dataPath.Length);
        ModelImporter modelImporter = AssetImporter.GetAtPath(relativePath) as ModelImporter;

        if (modelImporter == null)
        {
            Debug.LogError("The selected file is not a valid FBX.");
            return;
        }

        // Obtener clips de animación del FBX
        AnimationClip[] animationClips = AssetDatabase.LoadAllAssetsAtPath(relativePath)
            .Where(obj => obj is AnimationClip)
            .Cast<AnimationClip>()
            .ToArray();

        if (animationClips.Length == 0)
        {
            Debug.LogError("No animations found in the selected FBX.");
            return;
        }

        // Crear carpeta para guardar las animaciones extraídas
        string savePath = Path.GetDirectoryName(relativePath) + "/ExtractedAnimations/";
        if (!AssetDatabase.IsValidFolder(savePath))
        {
            AssetDatabase.CreateFolder(Path.GetDirectoryName(relativePath), "ExtractedAnimations");
        }

        // Guardar cada animación como un archivo .anim
        foreach (AnimationClip clip in animationClips)
        {
            string clipPath = savePath + clip.name + ".anim";
            AnimationClip newClip = new AnimationClip();
            EditorUtility.CopySerialized(clip, newClip);
            AssetDatabase.CreateAsset(newClip, clipPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Extracted {animationClips.Length} animations to: {savePath}");
    }
}

