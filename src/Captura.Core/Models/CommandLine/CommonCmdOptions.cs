using CommandLine;

namespace Captura
{
    public class CommonCmdOptions : CmdOptionsBase
    {
        public CommonCmdOptions()
        {
            Cursor = Settings.Instance.IncludeCursor;

            Keys = Settings.Instance.KeyStrokes;

            Clicks = Settings.Instance.MouseClicks;
        }

        [Option("reset", DefaultValue = false, HelpText = "Reset all setting values to default.")]
        public bool Reset { get; set; }

        [Option("tray", DefaultValue = false, HelpText = "Start minimized into the system tray.")]
        public virtual bool Tray { get; set; }

        [Option("delay", DefaultValue = 0, HelpText = "Milliseconds to wait before starting recording.")]
        public int Delay { get; set; }

        [Option("length", DefaultValue = 0, HelpText = "Length of Recording in seconds.")]
        public virtual int Length { get; set; }

        [Option("cursor", HelpText = "Include Cursor in Recording.")]
        public bool? Cursor { get; set; }

        [Option("keys", HelpText = "Include Keystrokes in Recording.")]
        public bool? Keys { get; set; }

        [Option("clicks", HelpText = "Include Mouse Clicks in Recording.")]
        public bool? Clicks { get; set; }
    }
}
