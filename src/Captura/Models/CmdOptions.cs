using CommandLine;

namespace Captura
{
    /// <summary>
    /// Command-line options for the WPF app.
    /// </summary>
    public class CmdOptions
    {
        [Option("reset", HelpText = "Reset all setting values to default.")]
        public bool Reset { get; set; }

        [Option("tray", HelpText = "Start minimized into the system tray.")]
        public bool Tray { get; set; }
        
        [Option("no-hotkey", HelpText = "Do not Register hotkeys.")]
        public bool NoHotkeys { get; set; }

        [Option("no-persist", HelpText = "Do not save any changes in settings.")]
        public bool NoPersist { get; set; }

        [Option("settings", HelpText = "Settings Directory")]
        public string Settings { get; set; }
    }
}
