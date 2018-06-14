using System.IO;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once InconsistentNaming
    public class FFMpegPostProcessingWriter : IVideoFileWriter
    {
        readonly IVideoFileWriter _ffMpegWriter;
        readonly string _tempFileName;
        readonly FFMpegVideoWriterArgs _args;

        public FFMpegPostProcessingWriter(FFMpegVideoWriterArgs Args)
        {
            _args = Args;
            _tempFileName = Path.GetTempFileName();

            _ffMpegWriter = FFMpegItem.x264.GetVideoFileWriter(new VideoWriterArgs
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

            var videoInArgs = $"-i \"{_tempFileName}\"";
            var videoOutArgs = $"{_args.VideoArgsProvider(_args.VideoQuality)} -r {_args.FrameRate}";

            var audioOutArgs = "";

            if (_args.AudioProvider != null)
            {
                audioOutArgs = _args.AudioArgsProvider(_args.AudioQuality);
            }

            var process = FFMpegService.StartFFMpeg($"{videoInArgs} {videoOutArgs} {audioOutArgs} {_args.OutputArgs} \"{_args.FileName}\"", _args.FileName);

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
