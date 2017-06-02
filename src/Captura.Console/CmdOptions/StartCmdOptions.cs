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

        [Option("length", DefaultValue = 0, HelpText = "Length of Recording in seconds.")]
        public int Length { get; set; }
        
        [Option("keys", DefaultValue = false, HelpText = "Include Keystrokes in Recording.")]
        public bool Keys { get; set; }

        [Option("clicks", DefaultValue = false, HelpText = "Include Mouse Clicks in Recording.")]
        public bool Clicks { get; set; }

        [Option("mic", DefaultValue = -1, HelpText = "Index of Microphone source.")]
        public int Microphone { get; set; }

        [Option("speaker", DefaultValue = -1, HelpText = "Index of Speaker output source.")]
        public int Speaker { get; set; }
    }
}
