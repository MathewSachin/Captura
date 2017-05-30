using CommandLine;

namespace Captura
{
    public class ShotCmdOptions : CommonCmdOptions
    {
        [Option("tray", DefaultValue = true, HelpText = "Start minimized into the system tray.")]
        public override bool Tray { get; set; }
    }
}
