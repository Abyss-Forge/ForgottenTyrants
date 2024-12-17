using UnityEditor;
using UnityEngine;

[System.Serializable]
public class FolderReference
{
    [SerializeField] private string folderPath;
    public string FolderPath => folderPath;

#if UNITY_EDITOR
    /// <summary>
    /// Abre el cuadro de diálogo para seleccionar una carpeta y actualiza la ruta.
    /// </summary>
    public void SelectFolder()
    {
        string selectedPath = EditorUtility.SaveFolderPanel("Select Folder", "Assets", "");
        if (!string.IsNullOrEmpty(selectedPath))
        {
            // Asegurarse de convertir la ruta al formato relativo a "Assets"
            if (selectedPath.StartsWith(Application.dataPath))
            {
                folderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
            }
            else
            {
                Debug.LogWarning("Please select a folder inside the Assets directory.");
            }
        }
    }
#endif

}