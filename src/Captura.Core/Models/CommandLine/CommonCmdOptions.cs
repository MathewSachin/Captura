using CommandLine;

namespace Captura
{
    public class CommonCmdOptions : CmdOptionsBase
    {
        [Option("reset", DefaultValue = false, HelpText = "Reset all setting values to default.")]
        public bool Reset { get; set; }

        [Option("tray", DefaultValue = false, HelpText = "Start minimized into the system tray.")]
        public virtual bool Tray { get; set; }

        [Option("delay", DefaultValue = 0, HelpText = "Milliseconds to wait before starting recording.")]
        public int Delay { get; set; }

        [Option("length", DefaultValue = 0, HelpText = "Length of Recording in seconds.")]
        public virtual int Length { get; set; }
    }
}
