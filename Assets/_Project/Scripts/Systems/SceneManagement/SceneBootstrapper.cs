using UnityEngine;
using Systems.SingletonPattern;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Systems.SceneManagement
{
    public class SceneBootstrapper : PersistentSingleton<SceneBootstrapper>
    {
        // NOTE: This script is intended to be placed in your first scene included in the build settings.
#pragma warning disable CS0414  // The field is assigned but its value is never used
        static readonly int _sceneIndex = 0;
#pragma warning restore CS0414

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Debug.Log("Bootstrapping... ");
#if UNITY_EDITOR
            // Set the bootstrapper scene to be the play mode start scene when running in the editor
            // This will cause the bootstrapper scene to be loaded first (and only once) when entering
            // play mode from the Unity Editor, regardless of which scene is currently active.
#pragma warning disable CS0162 // Unreachable code detected
            if (GlobalConfig.SCENE_BOOTSTRAPPER_ENABLED)
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[_sceneIndex].path);
#pragma warning restore CS0162 
#endif
        }
    }
}