using UnityEngine;
using Utils.Extensions;

namespace Utils
{
    public static class DisplayHelper
    {

        public static void LoadPrefs()
        {
            int quality = PlayerPrefs.GetInt(PlayerPrefsKeys.DISPLAY_GRAPHICS_QUALITY);
            SetGraphicsQuality(quality);

            int width = PlayerPrefs.GetInt(PlayerPrefsKeys.DISPLAY_RESOLUTION_WIDTH);
            int height = PlayerPrefs.GetInt(PlayerPrefsKeys.DISPLAY_RESOLUTION_HEIGHT);
            SetResolution(width, height);

            int refreshRate = PlayerPrefs.GetInt(PlayerPrefsKeys.DISPLAY_REFRESH_RATE, 60);
            SetRefreshRate(refreshRate);

            bool fullScreen = PlayerPrefsExtensions.GetBool(PlayerPrefsKeys.DISPLAY_FULLSCREEN_MODE, true);
            SetFullscreen(fullScreen);
        }

        public static void SetGraphicsQuality(int index)
        {
            QualitySettings.SetQualityLevel(index);
            QualitySettings.vSyncCount = 0; //needed to prevent refresh rate to be unlimited

            PlayerPrefs.SetInt(PlayerPrefsKeys.DISPLAY_GRAPHICS_QUALITY, index);
        }

        public static void SetRefreshRate(int value)   //hz & fps
        {
            Application.targetFrameRate = value;

            PlayerPrefs.SetInt(PlayerPrefsKeys.DISPLAY_REFRESH_RATE, value);
        }

        public static void SetResolution(int width, int height)
        {
            Screen.SetResolution(width, height, Screen.fullScreenMode);

            PlayerPrefs.SetInt(PlayerPrefsKeys.DISPLAY_RESOLUTION_WIDTH, width);
            PlayerPrefs.SetInt(PlayerPrefsKeys.DISPLAY_RESOLUTION_HEIGHT, height);
        }

        public static void ToggleFullscreen() => SetFullscreen(!Screen.fullScreen);
        public static void SetFullscreen(bool enable)
        {
            FullScreenMode mode = enable ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            // Screen.fullScreenMode = mode;

            Screen.fullScreen = enable;
            if (!enable) CenterWindow();

            PlayerPrefsExtensions.SetBool(PlayerPrefsKeys.DISPLAY_FULLSCREEN_MODE, enable);
        }

        private static void CenterWindow()
        {
            Vector2Int screenCenter = new(Screen.width / 2, Screen.height / 2);
            Screen.MoveMainWindowTo(Screen.mainWindowDisplayInfo, screenCenter);
        }

    }
}