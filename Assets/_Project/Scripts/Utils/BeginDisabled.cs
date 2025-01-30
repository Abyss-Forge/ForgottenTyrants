using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BeginDisabled : MonoBehaviour
{
    public bool PauseForTesting;

    void OnValidate()
    {
        if (PauseForTesting) return;
        gameObject.SetActive(false);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BeginDisabled))]
public class AutoDisableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BeginDisabled autoDisable = (BeginDisabled)target;

        EditorGUILayout.HelpBox("I will start in a disabled state", MessageType.Info);

        if (autoDisable.PauseForTesting)
        {
            EditorGUILayout.HelpBox("Remember to uncheck test mode before building", MessageType.Warning);
        }

        DrawDefaultInspector();
    }
}
#endif
