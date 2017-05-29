using CommandLine;
using CommandLine.Text;

namespace Captura
{
    public class CommandLineOptions
    {
        //[Option('r', "read", Required = true, HelpText = "Input file to be processed.")]
        //public string InputFile { get; set; }

        // Reset settings
        // omitting long name, default --reset
        [Option(DefaultValue = false, HelpText = "Prints all messages to standard output.")]
        public bool Reset { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
