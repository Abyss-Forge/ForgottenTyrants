using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtils
{
    public static int Next => GetNext();
    public static int Current => GetCurrent();
    public static int Previous => GetPrevious();

    private static int GetNext()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }

    private static int GetCurrent()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    private static int GetPrevious()
    {
        int index = SceneManager.GetActiveScene().buildIndex - 1;
        if (index < 0) index = 0;
        return index;
    }

    public static string GetNameFromIndex(int index)
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(index);
        return Path.GetFileNameWithoutExtension(scenePath);
    }

    public static int GetIndexFromName(string name)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = GetNameFromIndex(i);
            if (name == sceneName)
            {
                return i;
            }
        }
        return -1;
    }

}