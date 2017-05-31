using CommandLine;

namespace Captura
{
    public class CommonCmdOptions : CmdOptionsBase
    {
        public CommonCmdOptions()
        {
            Settings.Instance.IncludeCursor = false;
        }
        
        [Option("cursor", DefaultValue = false, HelpText = "Include Cursor in Recording.")]
        public bool Cursor { get; set; }
    }
}
