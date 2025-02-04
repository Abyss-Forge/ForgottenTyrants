#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;

public class MultiplayerBuildAndRun : EditorWindow
{
    const string PLAYER_AMOUNT_KEY = "MB&R_PlayerAmount";
    const string PLATFORM_INDEX_KEY = "MB&R_SelectedPlatformIndex";
    const string RUN_IN_EDITOR_KEY = "MB&R_RunInEditor";

    int _playerAmount, _selectedPlatformIndex;
    bool _runInEditor;

    readonly BuildTarget[] _supportedBuildTargets = {
        BuildTarget.StandaloneWindows64,
        BuildTarget.StandaloneLinux64,
        BuildTarget.StandaloneOSX,
    };

    [MenuItem("File/Run Multiplayer")]
    public static void ShowWindow()
    {
        MultiplayerBuildAndRun window = GetWindow<MultiplayerBuildAndRun>("Run Multiplayer");
        window.maxSize = window.minSize = new Vector2(300, 160);
        window.LoadPreferences();
    }

    void OnGUI()
    {
        GUILayout.Label("Build a launcher for each player", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("Select target platform:", EditorStyles.label);

        string[] platformOptions = Array.ConvertAll(_supportedBuildTargets, target => target.ToString());
        _selectedPlatformIndex = EditorGUILayout.Popup(_selectedPlatformIndex, platformOptions);

        GUILayout.Space(10);
        _playerAmount = EditorGUILayout.IntField("Players:", _playerAmount);
        if (_playerAmount < 1) _playerAmount = 1;
        if (_playerAmount > 10) _playerAmount = 10;

        GUILayout.Space(10);
        _runInEditor = EditorGUILayout.Toggle("Also run in Editor", _runInEditor);

        GUILayout.Space(10);
        if (GUILayout.Button("Build"))
        {
            SavePreferences();
            PerformBuild();
            if (_runInEditor)
            {
                EditorApplication.isPlaying = true;
            }
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

    private void SavePreferences()
    {
        EditorPrefs.SetInt(PLAYER_AMOUNT_KEY, _playerAmount);
        EditorPrefs.SetInt(PLATFORM_INDEX_KEY, _selectedPlatformIndex);
        EditorPrefs.SetBool(RUN_IN_EDITOR_KEY, _runInEditor);
    }

    private void LoadPreferences()
    {
        _playerAmount = EditorPrefs.GetInt(PLAYER_AMOUNT_KEY, 2);
        _selectedPlatformIndex = EditorPrefs.GetInt(PLATFORM_INDEX_KEY, 0);
        _runInEditor = EditorPrefs.GetBool(RUN_IN_EDITOR_KEY, false);
    }
}

#endif