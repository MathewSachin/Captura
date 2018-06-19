using Screna;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Captura.Models
{
    /// <summary>
    /// Encode Video using FFmpeg.exe
    /// </summary>
    public class FFmpegWriter : IVideoFileWriter
    {
        readonly NamedPipeServerStream _audioPipe;

        readonly Process _ffmpegProcess;
        readonly NamedPipeServerStream _ffmpegIn;
        readonly byte[] _videoBuffer;

        const string PipePrefix = @"\\.\pipe\";

        static string GetPipeName() => $"captura-{Guid.NewGuid()}";

        /// <summary>
        /// Creates a new instance of <see cref="FFmpegWriter"/>.
        /// </summary>
        public FFmpegWriter(FFmpegVideoWriterArgs Args)
        {
            var settings = ServiceProvider.Get<Settings>();

            _videoBuffer = new byte[Args.ImageProvider.Width * Args.ImageProvider.Height * 4];

            var audioPipeName = GetPipeName();
            var videoPipeName = GetPipeName();

            var videoInArgs = $"-framerate {Args.FrameRate} -f rawvideo -pix_fmt rgb32 -video_size {Args.ImageProvider.Width}x{Args.ImageProvider.Height} -i {PipePrefix}{videoPipeName}";
            var videoOutArgs = $"{Args.VideoArgsProvider(Args.VideoQuality)} -r {Args.FrameRate}";

            if (settings.FFmpeg.Resize)
            {
                var width = settings.FFmpeg.ResizeWidth;
                var height = settings.FFmpeg.ResizeHeight;

                if (width % 2 == 1)
                    ++width;

                if (height % 2 == 1)
                    ++height;

                videoOutArgs += $" -vf scale={width}:{height}";
            }

            string audioInArgs = "", audioOutArgs = "";
            
            if (Args.AudioProvider != null)
            {
                audioInArgs = $"-f s16le -acodec pcm_s16le -ar {Args.Frequency} -ac {Args.Channels} -i {PipePrefix}{audioPipeName}";
                audioOutArgs = Args.AudioArgsProvider(Args.AudioQuality);

                _audioPipe = new NamedPipeServerStream(audioPipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 10000, 10000);
            }

            _ffmpegIn = new NamedPipeServerStream(videoPipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 10000, 10000);

            _ffmpegProcess = FFmpegService.StartFFmpeg($"{videoInArgs} {audioInArgs} {videoOutArgs} {audioOutArgs} {Args.OutputArgs} \"{Args.FileName}\"", Args.FileName);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _ffmpegIn.Dispose();

            _audioPipe?.Dispose();

            _ffmpegProcess.WaitForExit();
        }

        /// <summary>
        /// Gets whether audio is supported.
        /// </summary>
        public bool SupportsAudio { get; } = true;

        bool _firstAudio = true;

        /// <summary>
        /// Write audio block to Audio Stream.
        /// </summary>
        /// <param name="Buffer">Buffer containing audio data.</param>
        /// <param name="Length">Length of audio data in bytes.</param>
        public void WriteAudio(byte[] Buffer, int Length)
        {
            if (_ffmpegProcess.HasExited)
            {
                throw new Exception("An Error Occurred with FFmpeg");
            }

            if (_firstAudio)
            {
                if (!_audioPipe.WaitForConnection(5000))
                {
                    throw new Exception("Cannot connect Audio pipe to FFmpeg");
                }

                _firstAudio = false;
            }

            _audioPipe.WriteAsync(Buffer, 0, Length);
        }

        bool _firstFrame = true;

        Task _lastFrameTask;

        /// <summary>
        /// Writes an Image frame.
        /// </summary>
        public void WriteFrame(IBitmapFrame Frame)
        {
            if (_ffmpegProcess.HasExited)
            {
                Frame.Dispose();
                throw new Exception($"An Error Occurred with FFmpeg, Exit Code: {_ffmpegProcess.ExitCode}");
            }
            
            if (_firstFrame)
            {
                if (!_ffmpegIn.WaitForConnection(5000))
                {
                    throw new Exception("Cannot connect Video pipe to FFmpeg");
                }

                _firstFrame = false;
            }

            _lastFrameTask?.Wait();

            if (!(Frame is RepeatFrame))
            {
                using (Frame)
                {
                    Frame.CopyTo(_videoBuffer, _videoBuffer.Length);
                }
            }
            
            _lastFrameTask = _ffmpegIn.WriteAsync(_videoBuffer, 0, _videoBuffer.Length);
        }
    }
}
