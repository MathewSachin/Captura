using CommandLine;

namespace Captura
{
    [Verb("ffmpeg", HelpText = "Manage FFmpeg")]
    // ReSharper disable once ClassNeverInstantiated.Global
    class FFmpegCmdOptions : ICmdlineVerb
    {
        [Option("install", HelpText = "Install FFmpeg to specified folder.")]
        public string Install { get; set; }

        public void Run()
        {
            var ffmpegManager = ServiceProvider.Get<FFmpegConsoleManager>();

            // Need to Wait instead of await otherwise the process will exit
            ffmpegManager.Run(this).Wait();
        }
    }
}
