using System;
using System.Threading.Tasks;
using Captura.Video;

namespace Captura.FFmpeg
{
    // ReSharper disable once InconsistentNaming
    class FFmpegVideoConverter : IVideoConverter
    {
        readonly FFmpegVideoCodec _videoCodec;

        public FFmpegVideoConverter(FFmpegVideoCodec VideoCodec)
        {
            _videoCodec = VideoCodec;
        }

        public string Name => $"{_videoCodec.Name} (FFmpeg)";

        public string Extension => _videoCodec.Extension;

        public async Task StartAsync(VideoConverterArgs Args, IProgress<int> Progress)
        {
            if (!FFmpegService.FFmpegExists)
            {
                throw new FFmpegNotFoundException();
            }

            var argsBuilder = new FFmpegArgsBuilder();

            argsBuilder.AddInputFile(Args.InputFile);

            var output = argsBuilder.AddOutputFile(Args.FileName)
                .SetFrameRate(Args.FrameRate);

            _videoCodec.Apply(ServiceProvider.Get<FFmpegSettings>(), Args, output);

            //if (Args.AudioProvider != null)
            {
                _videoCodec.AudioArgsProvider(Args.AudioQuality, output);
            }

            var process = FFmpegService.StartFFmpeg(argsBuilder.GetArgs(), Args.FileName, out var log);

            log.ProgressChanged += Progress.Report;

            await Task.Run(() => process.WaitForExit());

            if (process.ExitCode != 0)
                throw new FFmpegException(process.ExitCode);

            Progress.Report(100);
        }
    }
}
