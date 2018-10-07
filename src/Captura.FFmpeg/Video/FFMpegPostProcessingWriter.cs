using System.IO;

namespace Captura.Models
{
    // ReSharper disable once InconsistentNaming
    public class FFmpegPostProcessingWriter : IVideoFileWriter
    {
        readonly IVideoFileWriter _ffMpegWriter;
        readonly string _tempFileName;
        readonly FFmpegVideoWriterArgs _args;

        public FFmpegPostProcessingWriter(FFmpegVideoWriterArgs Args)
        {
            _args = Args;
            _tempFileName = Path.GetTempFileName();

            _ffMpegWriter = FFmpegItem.x264.GetVideoFileWriter(new VideoWriterArgs
            {
                AudioProvider = Args.AudioProvider,
                AudioQuality = Args.AudioQuality,
                FileName = _tempFileName,
                FrameRate = Args.FrameRate,
                ImageProvider = Args.ImageProvider,
                VideoQuality = Args.VideoQuality
            }, "-f mp4 -y");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _ffMpegWriter.Dispose();

            var argsBuilder = new FFmpegArgsBuilder();

            argsBuilder.AddInputFile(_tempFileName);

            var output = argsBuilder.AddOutputFile(_args.FileName)
                .AddArg(_args.VideoArgsProvider(_args.VideoQuality))
                .SetFrameRate(_args.FrameRate);

            if (_args.AudioProvider != null)
            {
                output.AddArg(_args.AudioArgsProvider(_args.AudioQuality));
            }

            output.AddArg(_args.OutputArgs);

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
