using System.IO;
using UnityEngine.SceneManagement;

namespace Utils
{
    public static class SceneUtils
    {
        public static int Next => GetNext();
        public static int Current => GetCurrent();
        public static int Previous => GetPrevious();

        private static int GetNext()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            if (index < SceneManager.sceneCountInBuildSettings - 1) index += 1;
            return index;
        }

        private static int GetCurrent()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }

        private static int GetPrevious()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            if (index > 0) index -= 1;
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
}