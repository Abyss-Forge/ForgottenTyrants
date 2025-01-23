using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshExtract : MonoBehaviour
{
    [MenuItem("Tools/Extract Mesh From FBX")]
    public static void ExtractMesh()
    {
        // Verifica si hay un objeto seleccionado en el Project
        if (Selection.activeObject == null)
        {
            Debug.LogError("No asset selected. Please select a FBX or 3D model asset in the Project window.");
            return;
        }

        // Obtén el modelo como GameObject
        GameObject selectedObject = Selection.activeObject as GameObject;

        if (selectedObject == null)
        {
            Debug.LogError("The selected asset is not a GameObject. Please select a valid FBX or model asset.");
            return;
        }

        // Busca el MeshFilter o SkinnedMeshRenderer en el modelo
        MeshFilter meshFilter = selectedObject.GetComponentInChildren<MeshFilter>();
        SkinnedMeshRenderer skinnedMeshRenderer = selectedObject.GetComponentInChildren<SkinnedMeshRenderer>();
        Mesh mesh = null;

        if (meshFilter != null)
        {
            mesh = meshFilter.sharedMesh;
        }
        else if (skinnedMeshRenderer != null)
        {
            mesh = skinnedMeshRenderer.sharedMesh;
        }

        if (mesh == null)
        {
            Debug.LogError("No mesh found in the selected asset.");
            return;
        }

        // Solicita al usuario una ruta para guardar el asset
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Mesh",
            mesh.name + ".asset",
            "asset",
            "Save the extracted mesh as an asset."
        );

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("Save operation canceled or invalid path.");
            return;
        }

        // Intenta guardar la malla como un asset
        try
        {
            AssetDatabase.CreateAsset(Object.Instantiate(mesh), path);
            AssetDatabase.SaveAssets();
            Debug.Log("Mesh successfully saved to: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save mesh: {e.Message}");
        }
    }
}
