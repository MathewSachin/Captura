using System;
using System.IO;
using System.Threading.Tasks;

namespace Captura.Models
{
    // ReSharper disable once InconsistentNaming
    class FFmpegGifConverter : IVideoConverter
    {
        async Task<string> GeneratePalette(VideoConverterArgs Args)
        {
            var argsBuilder = new FFmpegArgsBuilder();

            argsBuilder.AddInputFile(Args.InputFile);

            var tempFile = Path.GetTempFileName();
            var paletteFile = Path.ChangeExtension(tempFile, "png");
            File.Move(tempFile, paletteFile);

            argsBuilder.AddOutputFile(paletteFile)
                .AddArg("vf", "palettegen")
                .SetFrameRate(Args.FrameRate)
                .AddArg("-y");

            var process = FFmpegService.StartFFmpeg(argsBuilder.GetArgs(), paletteFile, out _);

            await Task.Run(() => process.WaitForExit());

            return paletteFile;
        }

        async Task GenerateGif(string PaletteFile, VideoConverterArgs Args, IProgress<int> Progress)
        {
            var argsBuilder = new FFmpegArgsBuilder();

            argsBuilder.AddInputFile(Args.InputFile);

            argsBuilder.AddInputFile(PaletteFile);

            argsBuilder.AddOutputFile(Args.FileName)
                .AddArg("lavfi", "paletteuse")
                .SetFrameRate(Args.FrameRate);

            var process = FFmpegService.StartFFmpeg(argsBuilder.GetArgs(), Args.FileName, out var log);
            log.ProgressChanged += M => Progress.Report(M);

            await Task.Run(() => process.WaitForExit());
        }

        public string Name => "Gif (FFmpeg)";

        public string Extension => ".gif";

        public async Task StartAsync(VideoConverterArgs Args, IProgress<int> Progress)
        {
            var palletteFile = await GeneratePalette(Args);

            await GenerateGif(palletteFile, Args, Progress);

            Progress.Report(100);

            File.Delete(palletteFile);
        }
    }
}
