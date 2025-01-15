using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RememberToRemoveMe : MonoBehaviour { }

#if UNITY_EDITOR
[CustomEditor(typeof(RememberToRemoveMe))]
public class RememberToRemoveMeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Mostrar un mensaje en el inspector
        EditorGUILayout.HelpBox("Remember to remove me!", MessageType.Warning);

        // Llamar al inspector predeterminado para no perder funcionalidades
        DrawDefaultInspector();
    }
}
#endif
