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
        /// Get the Window selected in the Window list (Func<Window>).
        /// </summary>
        SelectedWindow,
        
        /// <summary>
        /// Get the IRegionProvider.
        /// </summary>
        RegionProvider,
        
        /// <summary>
        /// Minimize Main Window (Action<bool>).
        /// </summary>
        Minimize,
        
        /// <summary>
        /// Gets the ISystemTray.
        /// </summary>
        SystemTray,

        /// <summary>
        /// Gets the IMessageProvider.
        /// </summary>
        Message,

        /// Gets the IWebCamProvider.
        /// </summary>
        WebCam,

        /// <summary>
        /// Set Main Window Visibility (Action<bool>).
        /// </summary>
        MainWindowVisibility
    }
}
