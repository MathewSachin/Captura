using CommandLine;

namespace Captura
{
    public class VerbCmdOptions : CmdOptionsBase
    {
        [VerbOption("start")]
        public StartCmdOptions Start { get; set; }
    }
}
