using CommandLine;

namespace Captura
{
    [Verb("ffmpeg")]
    class FFMpegCmdOptions
    {
        [Option("install", HelpText = "Install FFMPeg to specified folder.")]
        public string Install { get; set; }
    }
}
