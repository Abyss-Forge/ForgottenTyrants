using UnityEditor;
using UnityEngine;

public static class ReferenceValidator
{

    public static void ValidateExisting(GameObject target, Object context = null)
    {
        if (target == null)
        {
            Debug.LogError("Empty field reference at: " + context);
            CancelExecution();
        }
    }

    public static void ValidateType<T>(GameObject target, Object context = null)
    {
        if (target.GetComponent<T>() == null)
        {
            Debug.LogError("Wrong class type at: " + context);
            CancelExecution();
        }
    }

    private static void CancelExecution()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

}
