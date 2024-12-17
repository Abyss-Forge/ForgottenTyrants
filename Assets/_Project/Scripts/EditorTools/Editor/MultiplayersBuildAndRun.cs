#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class MultiplayersBuildAndRun : EditorWindow
{
    private int _playerAmount = 2;

    [MenuItem("File/Run Multiplayer")]
    public static void ShowWindow()
    {
        GetWindow<MultiplayersBuildAndRun>("Run Multiplayer");
    }

    void OnGUI()
    {
        GUILayout.Label("Build a launcher for each player", EditorStyles.boldLabel);
        GUILayout.Space(10);

        _playerAmount = EditorGUILayout.IntField("Players:", _playerAmount);

        GUILayout.Space(10);
        if (GUILayout.Button("Build")) PerformWin64Build(_playerAmount);
    }

    private static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        for (int i = 1; i <= playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Win64/" + GetProjectName() + i.ToString() + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
        }
    }

    private static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    private static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

}

#endif