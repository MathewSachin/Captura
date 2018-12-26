using System.IO;

namespace Captura.Models
{
    // ReSharper disable once InconsistentNaming
    public class FFmpegGifWriter : IVideoFileWriter
    {
        readonly IVideoFileWriter _ffMpegWriter;
        readonly string _tempFileName;
        readonly VideoWriterArgs _args;

        public FFmpegGifWriter(VideoWriterArgs Args)
        {
            _args = Args;
            _tempFileName = Path.GetTempFileName();

            _ffMpegWriter = FFmpegItem.x264.GetVideoFileWriter(new VideoWriterArgs
            {
                FileName = _tempFileName,
                FrameRate = Args.FrameRate,
                ImageProvider = Args.ImageProvider,
                VideoQuality = Args.VideoQuality
            }, "-f mp4 -y");
        }

        string GeneratePalette()
        {
            var argsBuilder = new FFmpegArgsBuilder();

            argsBuilder.AddInputFile(_tempFileName);

            var tempFile = Path.GetTempFileName();
            var paletteFile = Path.ChangeExtension(tempFile, "png");
            File.Move(tempFile, paletteFile);

            argsBuilder.AddOutputFile(paletteFile)
                .AddArg("-vf palettegen")
                .SetFrameRate(_args.FrameRate)
                .AddArg("-y");

            var process = FFmpegService.StartFFmpeg(argsBuilder.GetArgs(), paletteFile);

            process.WaitForExit();

            return paletteFile;
        }

        void GenerateGif(string PaletteFile)
        {
            var argsBuilder = new FFmpegArgsBuilder();

            argsBuilder.AddInputFile(_tempFileName);

            argsBuilder.AddInputFile(PaletteFile);

            argsBuilder.AddOutputFile(_args.FileName)
                .AddArg("-lavfi paletteuse")
                .SetFrameRate(_args.FrameRate);

            var process = FFmpegService.StartFFmpeg(argsBuilder.GetArgs(), _args.FileName);

            process.WaitForExit();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _ffMpegWriter.Dispose();

            var palletteFile = GeneratePalette();

            GenerateGif(palletteFile);

            File.Delete(palletteFile);
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
