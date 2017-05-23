using Screna;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Captura.Models
{
    /// <summary>
    /// Encode Video using FFMpeg.exe
    /// </summary>
    public class FFMpegVideoWriter : IVideoFileWriter
    {
        readonly Process _ffmpegProcess;
        readonly Stream _ffmpegIn;

        // x264 requires video dimensions to be even.
        bool evenDimensions;
                
        /// <summary>
        /// Creates a new instance of <see cref="FFMpegVideoWriter"/>.
        /// </summary>
        /// <param name="FileName">Path for the output file.</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        public FFMpegVideoWriter(string FileName, int FrameRate, int Quality, FFMpegVideoArgsProvider VideoArgsProvider)
        {
            if (VideoArgsProvider == FFMpegItem.Mp4)
                evenDimensions = true;

            _ffmpegProcess = new Process
            {
                StartInfo =
                {
                    FileName = ServiceProvider.FFMpegExePath,
                    Arguments = $"-r {FrameRate} -f image2pipe -i - {VideoArgsProvider(Quality)} \"{FileName}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true
                }
            };

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
            if (evenDimensions)
            {
                var oldImage = Image;

                var rect = new Rectangle(Point.Empty, Image.Size);

                if (rect.Height % 2 == 1)
                    --rect.Height;
                
                if (rect.Width % 2 == 1)
                    --rect.Width;
                
                Image = Image.Clone(rect, Image.PixelFormat);

                oldImage.Dispose();
            }

            Image.Save(_ffmpegIn, ImageFormat.Png);
            Image.Dispose();
        }
    }
}
