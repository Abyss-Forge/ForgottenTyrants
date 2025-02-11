public sealed class PlayerPrefsKeys
{
    public const string INPUT_BINDINGS_JSON = "InputBindings";
    public const string TIME_SCALE = "TimeScale";

    #region Audio
    //Must have the same names as the AudioMixer entries
    public const string VOLUME_UI = "UI";
    public const string VOLUME_MASTER = "Master";
    public const string VOLUME_MUSIC = "Music";
    public const string VOLUME_SFX = "SFX";
    public const string VOLUME_CINEMATICS = "Cinematics";
    public const string VOLUME_DIALOG = "Dialog";
    public const string VOLUME_VOICE_CHAT = "VoiceChat";
    #endregion

    #region Display
    public const string DISPLAY_GRAPHICS_QUALITY = "GraphicsQuality";
    public const string DISPLAY_REFRESH_RATE = "RefreshRate";
    public const string DISPLAY_RESOLUTION_WIDTH = "ResolutionWidth";
    public const string DISPLAY_RESOLUTION_HEIGHT = "ResolutionHeight";
    public const string DISPLAY_FULLSCREEN_MODE = "FullscreenMode";
    #endregion
}