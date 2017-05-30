using CommandLine;

namespace Captura
{
    public class StartCmdOptions : CommonCmdOptions
    {
        [Option("tray", DefaultValue = true, HelpText = "Start minimized into the system tray.")]
        public override bool Tray { get; set; }
        
        [Option("length", Required = true, HelpText = "Length of Recording in seconds.")]
        public override int Length { get; set; }
    }
}
