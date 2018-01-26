using CommandLine;
using CommandLine.Text;

namespace Captura
{
    public class CmdOptionsBase
    {
        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, Current => HelpText.DefaultParsingErrorsHandler(this, Current));
        }
    }
}
