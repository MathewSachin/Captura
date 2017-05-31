using CommandLine;

namespace Captura
{
    public class StartCmdOptions : CommonCmdOptions
    {
        public StartCmdOptions()
        {
            Settings.Instance.KeyStrokes = Settings.Instance.MouseClicks = false;
        }

        [Option("delay", DefaultValue = 0, HelpText = "Milliseconds to wait before starting recording.")]
        public int Delay { get; set; }

        [Option("length", Required = true, HelpText = "Length of Recording in seconds.")]
        public int Length { get; set; }
        
        [Option("keys", DefaultValue = false, HelpText = "Include Keystrokes in Recording.")]
        public bool Keys { get; set; }

        [Option("clicks", DefaultValue = false, HelpText = "Include Mouse Clicks in Recording.")]
        public bool Clicks { get; set; }
    }
}
