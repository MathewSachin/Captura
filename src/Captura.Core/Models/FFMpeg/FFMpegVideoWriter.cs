﻿using Screna;
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
        readonly NamedPipeServerStream _audioPipe;

        readonly Process _ffmpegProcess;
        readonly NamedPipeServerStream _ffmpegIn;
        readonly byte[] _videoBuffer;

        const string PipePrefix = @"\\.\pipe\";

        static string GetPipeName() => $"captura-{Guid.NewGuid()}";

        /// <summary>
        /// Creates a new instance of <see cref="FFMpegWriter"/>.
        /// </summary>
        public FFMpegWriter(FFMpegVideoWriterArgs Args)
        {
            var settings = ServiceProvider.Get<Settings>();

            _videoBuffer = new byte[Args.ImageProvider.Width * Args.ImageProvider.Height * 4];

            var audioPipeName = GetPipeName();
            var videoPipeName = GetPipeName();

            var videoInArgs = $"-framerate {Args.FrameRate} -f rawvideo -pix_fmt rgb32 -video_size {Args.ImageProvider.Width}x{Args.ImageProvider.Height} -i {PipePrefix}{videoPipeName}";
            var videoOutArgs = $"{Args.VideoArgsProvider(Args.VideoQuality)} -r {Args.FrameRate}";

            if (settings.FFMpeg.Resize)
            {
                var width = settings.FFMpeg.ResizeWidth;
                var height = settings.FFMpeg.ResizeHeight;

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

            _ffmpegProcess = FFMpegService.StartFFMpeg($"{videoInArgs} {audioInArgs} {videoOutArgs} {audioOutArgs} {Args.OutputArgs} \"{Args.FileName}\"");
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
                throw new Exception("An Error Occured with FFMpeg");
            }

            if (_firstAudio)
            {
                _audioPipe.WaitForConnection();

                _firstAudio = false;
            }

            _audioPipe.WriteAsync(Buffer, 0, Length);
        }

        bool _firstFrame = true;

        /// <summary>
        /// Writes an Image frame.
        /// </summary>
        public void WriteFrame(IBitmapFrame Frame)
        {
            if (_ffmpegProcess.HasExited)
            {
                Frame.Dispose();
                throw new Exception($"An Error Occured with FFMpeg, Exit Code: {_ffmpegProcess.ExitCode}");
            }
            
            if (_firstFrame)
            {
                _ffmpegIn.WaitForConnection();

                _firstFrame = false;
            }

            if (!(Frame is RepeatFrame))
            {
                using (Frame)
                {
                    var image = Frame.Bitmap;

                    var bits = image.LockBits(new Rectangle(Point.Empty, image.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(bits.Scan0, _videoBuffer, 0, _videoBuffer.Length);
                    image.UnlockBits(bits);
                }
            }
            
            _ffmpegIn.WriteAsync(_videoBuffer, 0, _videoBuffer.Length);
        }
    }
}
