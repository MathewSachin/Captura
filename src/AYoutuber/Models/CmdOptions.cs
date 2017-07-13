using CommandLine;

namespace Captura
{
    public class CmdOptions
    {
        [Option("reset", DefaultValue = false, HelpText = "Reset all setting values to default.")]
        public bool Reset { get; set; }

        [Option("tray", DefaultValue = false, HelpText = "Start minimized into the system tray.")]
        public bool Tray { get; set; }
        
        [Option("no-hotkey", DefaultValue = false, HelpText = "Do not Register hotkeys.")]
        public bool NoHotkeys { get; set; }

        [Option("no-persist", DefaultValue = false, HelpText = "Do not save any changes in settings.")]
        public bool NoPersist { get; set; }
    }
}
