#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class RectTransformSizeEditor : EditorWindow
{
    private RectTransform _selectedRectTransform;
    private float _percentage = 100f;

    [MenuItem("CONTEXT/RectTransform/SizeScaler", priority = 50)]
    public static void ShowWindow()
    {
        RectTransformSizeEditor window = GetWindow<RectTransformSizeEditor>("RectTransform Size Editor");
        window.minSize = new Vector2(250, 100);
    }

    private void OnGUI()
    {
        GUILayout.Label("Set RectTransform Size", EditorStyles.boldLabel);

        GUILayout.Space(10);

        _selectedRectTransform = Selection.activeGameObject?.GetComponent<RectTransform>();
        if (_selectedRectTransform == null)
        {
            GUILayout.Label("Select a RectTransform in the hierarchy.");
            return;
        }

        _percentage = EditorGUILayout.FloatField("Percentage (%):", _percentage);

        GUILayout.Space(10);

        if (GUILayout.Button("Set Size"))
        {
            SetRectTransformSize(_selectedRectTransform, _percentage / 100f);
        }

        minSize = new Vector2(250, GUILayoutUtility.GetLastRect().yMax + 20);
    }

    private void SetRectTransformSize(RectTransform rectTransform, float percentage)
    {
        if (rectTransform.parent == null) return;

        RectTransform parentRectTransform = rectTransform.parent as RectTransform;
        Vector2 parentSize = parentRectTransform.rect.size;

        Vector2 newSize = new Vector2(parentSize.x * percentage, parentSize.y * percentage);

        Undo.RecordObject(rectTransform, "Size Modification");

        rectTransform.sizeDelta = newSize;
    }
}

#endif