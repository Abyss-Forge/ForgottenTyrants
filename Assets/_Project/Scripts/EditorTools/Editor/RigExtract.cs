using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RigExtract : MonoBehaviour
{
    [MenuItem("Tools/Extract Avatar from FBX")]
    static void ExtractAvatar()
    {
        // Seleccionar un FBX en la carpeta Assets
        string fbxPath = EditorUtility.OpenFilePanel("Select FBX", Application.dataPath, "fbx");
        if (string.IsNullOrEmpty(fbxPath)) return;

        string relativePath = "Assets" + fbxPath.Substring(Application.dataPath.Length);
        ModelImporter modelImporter = AssetImporter.GetAtPath(relativePath) as ModelImporter;

        if (modelImporter == null)
        {
            Debug.LogError("The selected file is not a valid FBX.");
            return;
        }

        // Asegurar que el tipo de animación sea "Humanoid"
        modelImporter.animationType = ModelImporterAnimationType.Human;
        AssetDatabase.ImportAsset(relativePath);

        // Obtener el Avatar
        GameObject fbxModel = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);
        if (fbxModel == null)
        {
            Debug.LogError("No valid FBX Model found.");
            return;
        }

        Avatar avatar = fbxModel.GetComponent<Animator>()?.avatar;
        if (avatar == null || !avatar.isHuman)
        {
            Debug.LogError("No valid Humanoid Avatar found in the FBX.");
            return;
        }

        // Crear una copia del Avatar para evitar el error de duplicación
        Avatar newAvatar = Object.Instantiate(avatar);
        newAvatar.name = avatar.name + "_Extracted";

        // Guardar el Avatar en una carpeta separada
        string savePath = Path.GetDirectoryName(relativePath) + "/ExtractedAvatars/";
        if (!AssetDatabase.IsValidFolder(savePath))
        {
            AssetDatabase.CreateFolder(Path.GetDirectoryName(relativePath), "ExtractedAvatars");
        }

        string avatarPath = savePath + newAvatar.name + ".asset";
        AssetDatabase.CreateAsset(newAvatar, avatarPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Avatar extracted and saved at: {avatarPath}");
    }
}
