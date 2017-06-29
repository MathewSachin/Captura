using Screna;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Captura.Models
{
    /// <summary>
    /// Encode Video using FFMpeg.exe
    /// </summary>
    public class FFMpegVideoWriter : IVideoFileWriter
    {
        readonly Process _ffmpegProcess;
        readonly Stream _ffmpegIn;
        readonly byte[] _videoBuffer;
        bool _exited;
        
        /// <summary>
        /// Creates a new instance of <see cref="FFMpegVideoWriter"/>.
        /// </summary>
        /// <param name="FileName">Path for the output file.</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        public FFMpegVideoWriter(string FileName, IImageProvider ImageProvider, int FrameRate, int Quality, FFMpegVideoArgsProvider VideoArgsProvider)
        {
            _videoBuffer = new byte[ImageProvider.Width * ImageProvider.Height * 4];

            _ffmpegProcess = new Process
            {
                StartInfo =
                {
                    FileName = ServiceProvider.FFMpegExePath,
                    Arguments = $"-hide_banner -framerate {FrameRate} -f rawvideo -pix_fmt rgb32 -video_size {ImageProvider.Width}x{ImageProvider.Height} -i - {VideoArgsProvider(Quality)} -r {FrameRate} \"{FileName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true
                }
            };

            _ffmpegProcess.Exited += (s, e) => _exited = true;

            _ffmpegProcess.Start();

            _ffmpegIn = _ffmpegProcess.StandardInput.BaseStream;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _ffmpegIn.Flush();
            _ffmpegIn.Close();
            _ffmpegProcess.WaitForExit();
        }

        /// <summary>
        /// Gets whether audio is supported.
        /// </summary>
        public bool SupportsAudio { get; } = false;
        
        /// <summary>
        /// Write audio block to Audio Stream.
        /// </summary>
        /// <param name="Buffer">Buffer containing audio data.</param>
        /// <param name="Length">Length of audio data in bytes.</param>
        public void WriteAudio(byte[] Buffer, int Length) { }
        
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
            
            var bits = Image.LockBits(new Rectangle(Point.Empty, Image.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            Marshal.Copy(bits.Scan0, _videoBuffer, 0, _videoBuffer.Length);
            Image.UnlockBits(bits);

            Image.Dispose();

            _ffmpegIn.Write(_videoBuffer, 0, _videoBuffer.Length);
        }
    }
}
