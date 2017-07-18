using Screna;
using Screna.Audio;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Pipes;
using System.Runtime.InteropServices;

namespace Captura.Models
{
    /// <summary>
    /// Encode Video using FFMpeg.exe
    /// </summary>
    public class FFMpegWriter : IVideoFileWriter
    {
        const string AudioPipeName = "capturaaudio";
        const string VideoPipeName = "capturavideo";
        readonly NamedPipeServerStream _audioPipe;

        readonly Process _ffmpegProcess;
        readonly NamedPipeServerStream _ffmpegIn;
        readonly byte[] _videoBuffer;
        bool _exited;
        
        /// <summary>
        /// Creates a new instance of <see cref="FFMpegWriter"/>.
        /// </summary>
        /// <param name="FileName">Path for the output file.</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        public FFMpegWriter(string FileName, IImageProvider ImageProvider, int FrameRate,
            int VideoQuality, FFMpegVideoArgsProvider VideoArgsProvider,
            int AudioQuality, FFMpegAudioArgsProvider AudioArgsProvider,
            IAudioProvider AudioProvider, int Frequency = 44100, int Channels = 2)
        {
            _videoBuffer = new byte[ImageProvider.Width * ImageProvider.Height * 4];

            var pipe = @"\\.\pipe\";

            var videoInArgs = $"-framerate {FrameRate} -f rawvideo -pix_fmt rgb32 -video_size {ImageProvider.Width}x{ImageProvider.Height} -i {pipe}{VideoPipeName}";
            var videoOutArgs = $"{VideoArgsProvider(VideoQuality)} -r {FrameRate}";

            string audioInArgs = "", audioOutArgs = "";
            
            if (AudioProvider != null)
            {
                audioInArgs = $"-f s16le -acodec pcm_s16le -ar {Frequency} -ac {Channels} -i {pipe}{AudioPipeName}";
                audioOutArgs = AudioArgsProvider(AudioQuality);

                _audioPipe = new NamedPipeServerStream(AudioPipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 10000, 10000);
            }

            _ffmpegIn = new NamedPipeServerStream(VideoPipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 10000, 10000);

            _ffmpegProcess = new Process
            {
                StartInfo =
                {
                    FileName = FFMpegService.FFMpegExePath,
                    Arguments = $"{videoInArgs} {audioInArgs} {videoOutArgs} {audioOutArgs} \"{FileName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                }
            };

            _ffmpegProcess.Exited += (s, e) => _exited = true;

            _ffmpegProcess.Start();

            _ffmpegIn.WaitForConnection();

            FFMpegLog.Instance.ReadLog(_ffmpegProcess.StandardError, "frame=");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _ffmpegIn.Flush();
            _ffmpegIn.Close();

            _audioPipe?.Flush();
            _audioPipe?.Dispose();

            _ffmpegProcess.WaitForExit();
        }

        /// <summary>
        /// Gets whether audio is supported.
        /// </summary>
        public bool SupportsAudio { get; } = true;

        bool firstAudio = true;

        /// <summary>
        /// Write audio block to Audio Stream.
        /// </summary>
        /// <param name="Buffer">Buffer containing audio data.</param>
        /// <param name="Length">Length of audio data in bytes.</param>
        public void WriteAudio(byte[] Buffer, int Length)
        {
            if (_exited)
            {
                throw new Exception("An Error Occured with FFMpeg");
            }

            if (firstAudio)
            {
                _audioPipe.WaitForConnection();

                firstAudio = false;
            }

            _audioPipe.WriteAsync(Buffer, 0, Length);
        }

        int lastFrameHash;
        
        /// <summary>
        /// Writes an Image frame.
        /// </summary>
        /// <param name="Image">The Image frame to write.</param>
        public void WriteFrame(Bitmap Image)
        {
            if (_exited)
            {
                Image.Dispose();
                throw new Exception($"An Error Occured with FFMpeg, Exit Code: {_ffmpegProcess.ExitCode}");
            }

            var hash = Image.GetHashCode();

            if (lastFrameHash != hash)
            {
                using (Image)
                {
                    var bits = Image.LockBits(new Rectangle(Point.Empty, Image.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(bits.Scan0, _videoBuffer, 0, _videoBuffer.Length);
                    Image.UnlockBits(bits);
                }

                lastFrameHash = hash;
            }
            
            _ffmpegIn.WriteAsync(_videoBuffer, 0, _videoBuffer.Length);
        }
    }
}
