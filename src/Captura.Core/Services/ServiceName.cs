﻿namespace Captura
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
        /// Get MainWindow Location (Func<Point>).
        /// </summary>
        MainWindowLocation,

        /// <summary>
        /// Set MainWindow Location (Action<Point>).
        /// </summary>
        SetMainWindowLocation,

        /// <summary>
        /// Gets the ISystemTray.
        /// </summary>
        SystemTray
    }
}
