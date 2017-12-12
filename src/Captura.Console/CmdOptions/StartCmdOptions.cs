using CommandLine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StartCmdOptions : CommonCmdOptions
    {
        public StartCmdOptions()
        {
            Settings.Instance.Keystrokes.Display = Settings.Instance.Clicks.Display = false;

            FrameRate = Settings.Instance.FrameRate;
            VideoQuality = Settings.Instance.VideoQuality;
            AudioQuality = Settings.Instance.AudioQuality;
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

        [Option('r', "framerate", HelpText = "Recording frame rate.")]
        public int FrameRate { get; set; }

        [Option("encoder", DefaultValue = null, HelpText = "Video encoder to use.")]
        public string Encoder { get; set; }

        [Option("vq", HelpText = "Video Quality")]
        public int VideoQuality { get; set; }

        [Option("aq", HelpText = "Audio Quality")]
        public int AudioQuality { get; set; }
    }
}
