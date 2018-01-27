using CommandLine;

namespace Captura
{
    abstract class CommonCmdOptions
    {
        protected CommonCmdOptions()
        {
            var settings = ServiceProvider.Get<Settings>();

            settings.IncludeCursor = false;
        }
        
        [Option("cursor", HelpText = "Include Cursor in Recording.")]
        public bool Cursor { get; set; }

        [Option("source", HelpText = "Video source")]
        public string Source { get; set; }

        [Option('f', "file", HelpText = "Output file path")]
        public string FileName { get; set; }
    }
}
