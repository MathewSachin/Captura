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
        [Option(DefaultValue = false, HelpText = "Reset all setting values to default.")]
        public bool Reset { get; set; }

        [Option(DefaultValue = false, HelpText = "Start minimized into the system tray.")]
        public bool Tray { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
