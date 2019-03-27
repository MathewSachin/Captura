using System.IO;
using Captura.FFmpeg;

namespace Captura.Models
{
    // ReSharper disable once InconsistentNaming
    class FFmpegPostProcessingWriter : IVideoFileWriter
    {
        readonly IVideoFileWriter _ffMpegWriter;
        readonly string _tempFileName;
        readonly FFmpegVideoWriterArgs _args;

        public FFmpegPostProcessingWriter(FFmpegVideoWriterArgs Args)
        {
            _args = Args;
            _tempFileName = Path.GetTempFileName();

            _ffMpegWriter = new TempFileVideoCodec().GetVideoFileWriter(new VideoWriterArgs
            {
                AudioProvider = Args.AudioProvider,
                AudioQuality = Args.AudioQuality,
                FileName = _tempFileName,
                FrameRate = Args.FrameRate,
                ImageProvider = Args.ImageProvider,
                VideoQuality = Args.VideoQuality
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _ffMpegWriter.Dispose();

            var argsBuilder = new FFmpegArgsBuilder();

            argsBuilder.AddInputFile(_tempFileName);

            var output = argsBuilder.AddOutputFile(_args.FileName)
                .SetFrameRate(_args.FrameRate);

            _args.VideoCodec.Apply(ServiceProvider.Get<FFmpegSettings>(), _args, output);

            if (_args.AudioProvider != null)
            {
                _args.VideoCodec.AudioArgsProvider(_args.AudioQuality, output);
            }

            var process = FFmpegService.StartFFmpeg(argsBuilder.GetArgs(), _args.FileName);

            process.WaitForExit();

            File.Delete(_tempFileName);
        }

        /// <inheritdoc />
        public bool SupportsAudio { get; } = true;
        
        /// <inheritdoc />
        public void WriteAudio(byte[] Buffer, int Length)
        {
            _ffMpegWriter.WriteAudio(Buffer, Length);
        }

        /// <inheritdoc />
        public void WriteFrame(IBitmapFrame Frame)
        {
            _ffMpegWriter.WriteFrame(Frame);
        }
    }
}
