using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Systems.SceneManagement
{
    public class SceneBootstrapper : PersistentSingleton<SceneBootstrapper>
    {
        // NOTE: This script is intended to be placed in your first scene included in the build settings.
        static readonly int _sceneIndex = 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Debug.Log("Bootstrapping... " + _sceneIndex);
#if UNITY_EDITOR
            // Set the bootstrapper scene to be the play mode start scene when running in the editor
            // This will cause the bootstrapper scene to be loaded first (and only once) when entering
            // play mode from the Unity Editor, regardless of which scene is currently active.
            if (GlobalConfig.SCENE_BOOTSTRAPPER_ENABLED)
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[_sceneIndex].path);
#endif
        }
    }
}