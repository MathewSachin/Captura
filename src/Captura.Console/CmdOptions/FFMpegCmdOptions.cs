using CommandLine;


namespace Captura
{
    public class FFMpegCmdOptions
    {
        [Option("install", DefaultValue = null, HelpText = "Install FFMPeg to specified folder.")]
        public string Install { get; set; }
    }
}
