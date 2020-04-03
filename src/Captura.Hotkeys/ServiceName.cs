namespace Captura.Hotkeys
{
    public enum ServiceName
    {
        None = -1,

        /// <summary>
        /// ScreenShot.
        /// </summary>
        ScreenShot,

        /// <summary>
        /// Start/Stop Recording.
        /// </summary>
        Recording,

        /// <summary>
        /// Pause/Resume Recording.
        /// </summary>
        Pause,

        /// <summary>
        /// ScreenShot of Desktop.
        /// </summary>
        DesktopScreenShot,

        /// <summary>
        /// ScreenShot of Active Window.
        /// </summary>
        ActiveScreenShot,

        /// <summary>
        /// Toggle Mouse clicks overlay.
        /// </summary>
        ToggleMouseClicks,

        /// <summary>
        /// Toggle Keystrokes overlay.
        /// </summary>
        ToggleKeystrokes,

        /// <summary>
        /// Screenshot Region (using Region Picker).
        /// </summary>
        ScreenShotRegion,

        /// <summary>
        /// ScreenShot using Screen Picker.
        /// </summary>
        ScreenShotScreen,

        /// <summary>
        /// ScreenShot using Window Picker.
        /// </summary>
        ScreenShotWindow,

        ShowMainWindow,

        ToggleRegionPicker
    }
}
