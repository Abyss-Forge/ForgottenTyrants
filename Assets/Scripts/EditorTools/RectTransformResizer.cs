#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class RectTransformSizeEditor : EditorWindow
{
    private RectTransform selectedRectTransform;
    private float percentage = 100f; // default percentaje

    [MenuItem("CONTEXT/RectTransform/SizeScaler", priority = 50)]
    public static void ShowWindow()
    {
        RectTransformSizeEditor window = GetWindow<RectTransformSizeEditor>("RectTransform Size Editor");
        window.minSize = new Vector2(250, 100); // Tamaño mínimo de la ventana
    }

    private void OnGUI()
    {
        GUILayout.Label("Set RectTransform Size", EditorStyles.boldLabel);

        // Espacio después del título
        GUILayout.Space(10);

        selectedRectTransform = Selection.activeGameObject?.GetComponent<RectTransform>();
        if (selectedRectTransform == null)
        {
            GUILayout.Label("Select a RectTransform in the hierarchy.");
            return;
        }

        percentage = EditorGUILayout.FloatField("Percentage (%):", percentage);

        // Espacio entre el campo de entrada y el botón
        GUILayout.Space(10);

        if (GUILayout.Button("Set Size"))
        {
            SetRectTransformSize(selectedRectTransform, percentage / 100f);
        }

        // Ajustar el tamaño de la ventana al contenido
        this.minSize = new Vector2(250, GUILayoutUtility.GetLastRect().yMax + 20);
    }

    private void SetRectTransformSize(RectTransform rectTransform, float percentage)
    {
        if (rectTransform.parent == null) return;

        // Obtener el RectTransform del padre
        RectTransform parentRectTransform = rectTransform.parent as RectTransform;
        Vector2 parentSize = parentRectTransform.rect.size;

        // Calcular el nuevo tamaño basado en el tamaño del padre
        Vector2 newSize = new Vector2(parentSize.x * percentage, parentSize.y * percentage);

        // Guardar el estado para deshacer
        Undo.RecordObject(rectTransform, "Size Modification");

        // Aplicar el nuevo tamaño
        rectTransform.sizeDelta = newSize;
    }
}

#endif