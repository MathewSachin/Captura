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
        /// Brings the MainWindow to Focus (Action).
        /// </summary>
        Focus,

        /// <summary>
        /// Exit Application (Action).
        /// </summary>
        Exit,

        /// <summary>
        /// Get RegionItem Video source (IVideoItem).
        /// </summary>
        RegionSource,

        /// <summary>
        /// Control visibility of Region Selector (Action<bool>).
        /// </summary>
        RegionSelectorVisibility,

        /// <summary>
        /// Get Region Rectangle (Func<Rectangle>).
        /// </summary>
        RegionRectangle,

        /// <summary>
        /// Set Region Rectangle (Action<Rectangle>).
        /// </summary>
        SetRegionRectangle,

        /// <summary>
        /// Minimize Main Window (Action<bool>).
        /// </summary>
        Minimize,

        /// <summary>
        /// Get MainWindow Location (Func<Point>).
        /// </summary>
        MainWindowLocation,

        /// <summary>
        /// Set MainWindow Location (Action<Point>).
        /// </summary>
        SetMainWindowLocation
    }
}
