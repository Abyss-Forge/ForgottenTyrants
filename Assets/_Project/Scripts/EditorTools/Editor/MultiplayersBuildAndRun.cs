#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;

public class MultiplayersBuildAndRun : EditorWindow
{
    private int _playerAmount = 2;
    private int _selectedPlatformIndex = 0;
    private readonly BuildTarget[] _supportedBuildTargets = {
        BuildTarget.StandaloneWindows64,
        BuildTarget.StandaloneOSX,
        BuildTarget.StandaloneLinux64
    };

    [MenuItem("File/Run Multiplayer")]
    public static void ShowWindow()
    {
        MultiplayersBuildAndRun window = GetWindow<MultiplayersBuildAndRun>("Run Multiplayer");
        window.maxSize = window.minSize = new Vector2(300, 130);
    }

    void OnGUI()
    {
        GUILayout.Label("Build a launcher for each player", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("Select Build Platform:", EditorStyles.label);

        string[] platformOptions = Array.ConvertAll(_supportedBuildTargets, target => target.ToString());
        _selectedPlatformIndex = EditorGUILayout.Popup(_selectedPlatformIndex, platformOptions);

        GUILayout.Space(10);
        _playerAmount = EditorGUILayout.IntField("Players:", _playerAmount);

        if (_playerAmount < 1) _playerAmount = 1;
        if (_playerAmount > 10) _playerAmount = 10;

        GUILayout.Space(10);
        if (GUILayout.Button("Build"))
        {
            PerformBuild();
            Close();
        }
    }

    private void PerformBuild()
    {
        BuildTarget selectedBuildTarget = _supportedBuildTargets[_selectedPlatformIndex];

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(selectedBuildTarget), selectedBuildTarget);

        for (int i = 1; i <= _playerAmount; i++)
        {
            string platformFolder = selectedBuildTarget.ToString();
            string outputPath = $"Builds/{platformFolder}/{GetProjectName()}_Player{i}.exe";
            BuildPipeline.BuildPlayer(GetScenePaths(), outputPath, selectedBuildTarget, BuildOptions.AutoRunPlayer);
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