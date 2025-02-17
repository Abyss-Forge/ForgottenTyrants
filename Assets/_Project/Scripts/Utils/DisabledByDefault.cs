using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DisabledByDefault : MonoBehaviour
{
    [Tooltip("This GameObject will be disabled on Validate before the game runs")]
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
        DisabledByDefault script = (DisabledByDefault)target;

        EditorGUILayout.HelpBox("I will be instantiated on disabled state", MessageType.Info);

        if (script.PauseForTesting)
        {
            EditorGUILayout.HelpBox("Remember to uncheck test mode before building", MessageType.Warning);
        }

        DrawDefaultInspector();
    }
}
#endif
