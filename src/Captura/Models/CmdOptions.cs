using CommandLine;

namespace Captura
{
    public class CmdOptions
    {
        [Option("reset", DefaultValue = false, HelpText = "Reset all setting values to default.")]
        public bool Reset { get; set; }

        [Option("tray", DefaultValue = false, HelpText = "Start minimized into the system tray.")]
        public virtual bool Tray { get; set; }
    }
}
