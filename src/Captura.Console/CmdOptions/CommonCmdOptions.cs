using CommandLine;

namespace Captura
{
    public abstract class CommonCmdOptions : CmdOptionsBase
    {
        protected CommonCmdOptions()
        {
            Settings.Instance.IncludeCursor = false;
        }
        
        [Option("cursor", DefaultValue = false, HelpText = "Include Cursor in Recording.")]
        public bool Cursor { get; set; }

        [Option("source", DefaultValue = null, HelpText = "Video source")]
        public string Source { get; set; }

        [Option('f', "file", DefaultValue = null, HelpText = "Output file path")]
        public string FileName { get; set; }
    }
}
