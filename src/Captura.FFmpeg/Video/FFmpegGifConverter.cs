using System;
using System.IO;
using System.Threading.Tasks;

namespace Captura.Models
{
    // ReSharper disable once InconsistentNaming
    class FFmpegGifConverter : IVideoConverter
    {
        public string Name => "Gif (FFmpeg)";

        public string Extension => ".gif";

        public async Task StartAsync(VideoConverterArgs Args, IProgress<int> Progress)
        {
            var argsBuilder = new FFmpegArgsBuilder();

            argsBuilder.AddInputFile(Args.InputFile);

            const string filter = "\"[0:v] split [a][b];[a] palettegen [p];[b][p] paletteuse\"";

            argsBuilder.AddOutputFile(Args.FileName)
                .AddArg("filter_complex", filter)
                .SetFrameRate(Args.FrameRate);

            var process = FFmpegService.StartFFmpeg(argsBuilder.GetArgs(), Args.FileName, out var log);
            log.ProgressChanged += M => Progress.Report(M);

            await Task.Run(() => process.WaitForExit());

            Progress.Report(100);
        }
    }
}
