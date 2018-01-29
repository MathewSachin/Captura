using CommandLine;

namespace Captura
{
    [Verb("ffmpeg", HelpText = "Manage FFMpeg")]
    class FFMpegCmdOptions
    {
        [Option("install", HelpText = "Install FFMpeg to specified folder.")]
        public string Install { get; set; }
    }
}
