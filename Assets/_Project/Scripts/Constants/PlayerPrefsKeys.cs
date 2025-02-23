public sealed class PlayerPrefsKeys
{
    public const string INPUT_BINDINGS_JSON = "InputBindings";
    public const string TIME_SCALE = "TimeScale";

    #region Audio
    //Must have the same names as the AudioMixer entries
    public const string VOLUME_UI = "UIVolume";
    public const string VOLUME_MASTER = "MasterVolume";
    public const string VOLUME_MUSIC = "MusicVolume";
    public const string VOLUME_SFX = "SFXVolume";
    public const string VOLUME_CINEMATICS = "CinematicsVolume";
    public const string VOLUME_DIALOG = "DialogVolume";
    public const string VOLUME_VOICE_CHAT = "VoiceChatVolume";
    #endregion

    #region Display
    public const string DISPLAY_GRAPHICS_QUALITY = "GraphicsQuality";
    public const string DISPLAY_REFRESH_RATE = "RefreshRate";
    public const string DISPLAY_RESOLUTION_WIDTH = "ResolutionWidth";
    public const string DISPLAY_RESOLUTION_HEIGHT = "ResolutionHeight";
    public const string DISPLAY_FULLSCREEN_MODE = "FullscreenMode";
    #endregion
}