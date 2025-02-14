using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DisabledByDefault : MonoBehaviour
{
    public bool PauseForTesting;

    void OnValidate()
    {
        if (PauseForTesting) return;
        gameObject.SetActive(false);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DisabledByDefault))]
public class DisabledByDefaultEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DisabledByDefault autoDisable = (DisabledByDefault)target;

        EditorGUILayout.HelpBox("I will start in a disabled state", MessageType.Info);

        if (autoDisable.PauseForTesting)
        {
            EditorGUILayout.HelpBox("Remember to uncheck test mode before building", MessageType.Warning);
        }

        DrawDefaultInspector();
    }
}
#endif
