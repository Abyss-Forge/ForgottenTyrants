using UnityEngine;

namespace Utils
{
    public static class TimeHelper
    {
        public static bool IsStopped => GetStopped();

        private static bool GetStopped()
        {
            return Time.timeScale == 0f;
        }

        public static void Stop()
        {
            if (IsStopped) return;

            float currentTimeScale = Time.timeScale;
            PlayerPrefs.SetFloat(PlayerPrefsKeys.TIME_SCALE, currentTimeScale);
            Time.timeScale = 0;
        }

        public static void Resume()
        {
            if (!IsStopped) return;

            float targetTimeScale = PlayerPrefs.GetFloat(PlayerPrefsKeys.TIME_SCALE, 1);
            Time.timeScale = targetTimeScale;
        }

        public static void SetScale(float time)
        {
            Time.timeScale = time;
        }

    }
}