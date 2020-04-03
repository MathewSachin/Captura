using System;
using System.Threading.Tasks;
using Captura.Video;

namespace Captura.FFmpeg
{
    // ReSharper disable once InconsistentNaming
    class FFmpegGifConverter : IVideoConverter
    {
        public string Name => "Gif (FFmpeg)";

        public string Extension => ".gif";

        public async Task StartAsync(VideoConverterArgs Args, IProgress<int> Progress)
        {
            if (!FFmpegService.FFmpegExists)
            {
                throw new FFmpegNotFoundException();
            }

            var argsBuilder = new FFmpegArgsBuilder();

            argsBuilder.AddInputFile(Args.InputFile);

            const string filter = "\"[0:v] split [a][b];[a] palettegen [p];[b][p] paletteuse\"";

            argsBuilder.AddOutputFile(Args.FileName)
                .AddArg("filter_complex", filter)
                .SetFrameRate(Args.FrameRate);

            var process = FFmpegService.StartFFmpeg(argsBuilder.GetArgs(), Args.FileName, out var log);
            log.ProgressChanged += Progress.Report;

            await Task.Run(() => process.WaitForExit());

            if (process.ExitCode != 0)
                throw new FFmpegException(process.ExitCode);

            Progress.Report(100);
        }
    }
}
