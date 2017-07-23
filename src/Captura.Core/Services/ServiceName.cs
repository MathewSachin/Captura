namespace Captura
{
    public enum ServiceName
    {
        /// <summary>
        /// ScreenShot (Action).
        /// </summary>
        ScreenShot,

        /// <summary>
        /// Start/Stop Recording (Action).
        /// </summary>
        Recording,

        /// <summary>
        /// Pause/Resume Recording (Action).
        /// </summary>
        Pause,

        /// <summary>
        /// ScreenShot of Desktop (Action).
        /// </summary>
        DesktopScreenShot,

        /// <summary>
        /// ScreenShot of Active Window (Action).
        /// </summary>
        ActiveScreenShot,

        /// <summary>
        /// Get the Window selected in the Window list (Func Window).
        /// </summary>
        SelectedWindow
    }
}
