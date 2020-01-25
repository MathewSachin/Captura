using System;
using System.IO;
using System.Threading.Tasks;
using Captura.FFmpeg;
using Captura.Gifski;

namespace Captura.Models
{
    class GifskiVideoConverter : IVideoConverter
    {
        public string Name => "Gif (Gifski)";

        public string Extension => ".gif";

        public async Task StartAsync(VideoConverterArgs Args, IProgress<int> Progress)
        {
            if (!FFmpegService.FFmpegExists)
            {
                throw new FFmpegNotFoundException();
            }

            var ffmpegArgs = new FFmpegArgsBuilder();
            ffmpegArgs.AddInputFile(Args.InputFile);

            var tempFolder = Path.Combine(Path.GetTempPath(), "Captura");
            Directory.CreateDirectory(tempFolder);
            var frameFormat = Path.Combine(tempFolder, "frame%04d.png");
            ffmpegArgs.AddOutputFile(frameFormat);

            using var ffmpeg = FFmpegService.StartFFmpeg(ffmpegArgs.GetArgs(), Args.FileName, out var log);
            log.ProgressChanged += p => Progress.Report(p / 2);

            await Task.Run(() => ffmpeg.WaitForExit());

            if (ffmpeg.ExitCode != 0)
                throw new FFmpegException(ffmpeg.ExitCode);

            var gifskiArgs = new GifskiArgsBuilder();
            gifskiArgs
                .AddInputFile(Path.Combine(tempFolder, "frame*.png"))
                .AddOutputFile(Args.FileName);

            using var gifski = GifskiService.StartGifski(
                gifskiArgs.GetArgs(), p => Progress.Report(p / 2 + 50));

            await Task.Run(() => gifski.WaitForExit());

            if (gifski.ExitCode != 0)
                throw new GifskiException(gifski.ExitCode);

            Directory.Delete(tempFolder, true);
        }
    }
}
