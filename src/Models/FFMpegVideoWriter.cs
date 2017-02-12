using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Screna.FFMpeg
{
    /// <summary>
    /// Encode Video using FFMpeg.exe
    /// </summary>
    public class FFMpegVideoWriter : IVideoFileWriter
    {
        /// <summary>
        /// Path to ffmpeg.exe
        /// </summary>
        public static string FFMpegPath { get; set; }

        readonly string _path, _outFile;
        static readonly Random Random = new Random();
        static readonly string BaseDir = Path.Combine(Path.GetTempPath(), "Screna.FFMpeg");
        int _fileIndex;
        readonly string _fileNameFormat;
        readonly int _frameRate;

        static FFMpegVideoWriter()
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);
        }

        /// <summary>
        /// Creates a new instance of <see cref="FFMpegVideoWriter"/>.
        /// </summary>
        /// <param name="FileName">Path for the output file... Output video type is determined by the file extension (e.g. ".avi", ".mp4", ".mov").</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        public FFMpegVideoWriter(string FileName, int FrameRate)
        {
            _outFile = FileName;
            _frameRate = FrameRate;

            int val;

            do val = Random.Next();
            while (Directory.Exists(Path.Combine(BaseDir, val.ToString())));

            _path = Path.Combine(BaseDir, val.ToString());
            Directory.CreateDirectory(_path);

            _fileNameFormat = Path.Combine(_path, "img-{0}.png");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var ffmpeg = File.Exists(FFMpegPath) ? FFMpegPath : "ffmpeg.exe";

            var p = Process.Start(ffmpeg, $"-r {_frameRate} -i {Path.Combine(_path, "img-%d.png")} {_outFile}");

            // TODO: Files are not deleted!!!
            p.Exited += (Sender, Args) => Directory.Delete(_path, true);
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
        public void WriteAudio(byte[] Buffer, int Length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes an Image frame.
        /// </summary>
        /// <param name="Image">The Image frame to write.</param>
        public void WriteFrame(Bitmap Image)
        {
            Image.Save(string.Format(_fileNameFormat, _fileIndex++), ImageFormat.Png);
        }
    }
}
