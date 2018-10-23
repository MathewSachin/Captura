using CommandLine;

namespace Captura
{
    [Verb("ffmpeg", HelpText = "Manage FFmpeg")]
    // ReSharper disable once ClassNeverInstantiated.Global
    class FFmpegCmdOptions
    {
        [Option("install", HelpText = "Install FFmpeg to specified folder.")]
        public string Install { get; set; }
    }
}
