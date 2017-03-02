using Screna;
using Screna.Audio;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Captura
{
    /// <summary>
    /// Encode Video using FFMpeg.exe
    /// </summary>
    public class FFMpegVideoWriter : IVideoFileWriter
    {
        readonly string _path;
        static readonly Random Random = new Random();
        static readonly string BaseDir = Path.Combine(Path.GetTempPath(), "Screna.FFMpeg");
        int _fileIndex;
        readonly string _fileNameFormat;
        AudioFileWriter _audioWriter;
        string _ffmpegArgs;

        static FFMpegVideoWriter()
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);
        }

        /// <summary>
        /// Creates a new instance of <see cref="FFMpegVideoWriter"/>.
        /// </summary>
        /// <param name="FileName">Path for the output file.</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        public FFMpegVideoWriter(string FileName, int FrameRate, FFMpegItem FFMpegItem, IAudioProvider AudioProvider = null)
        {
            int val;

            do val = Random.Next();
            while (Directory.Exists(Path.Combine(BaseDir, val.ToString())));

            _path = Path.Combine(BaseDir, val.ToString());
            Directory.CreateDirectory(_path);

            _fileNameFormat = Path.Combine(_path, "img-{0}.png");

            if (AudioProvider != null)
                _audioWriter = new AudioFileWriter(Path.Combine(_path, "audio.wav"), AudioProvider.WaveFormat);

            FFMpegItem.ArgsProvider(out var audioConfig, out var videoConfig);

            _ffmpegArgs = $"-r {FrameRate}";

            _ffmpegArgs += $" -i \"{Path.Combine(_path, "img-%d.png")}\"";

            if (AudioProvider != null)
                _ffmpegArgs += $" -i \"{Path.Combine(_path, "audio.wav")}\"";

            _ffmpegArgs += " " + videoConfig;

            if (AudioProvider != null)
                _ffmpegArgs += " " + audioConfig;

            _ffmpegArgs += $" \"{FileName}\"";
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _audioWriter?.Dispose();

            using (var p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "ffmpeg.exe";
                p.StartInfo.Arguments = _ffmpegArgs;
                p.Start();
                p.WaitForExit();
            }
            
            Directory.Delete(_path, true);
        }

        /// <summary>
        /// Gets whether audio is supported.
        /// </summary>
        public bool SupportsAudio { get; } = true;
        
        /// <summary>
        /// Write audio block to Audio Stream.
        /// </summary>
        /// <param name="Buffer">Buffer containing audio data.</param>
        /// <param name="Length">Length of audio data in bytes.</param>
        public void WriteAudio(byte[] Buffer, int Length)
        {
            _audioWriter?.Write(Buffer, 0, Length);
        }

        /// <summary>
        /// Writes an Image frame.
        /// </summary>
        /// <param name="Image">The Image frame to write.</param>
        public void WriteFrame(Bitmap Image)
        {
            Image.Save(string.Format(_fileNameFormat, ++_fileIndex), ImageFormat.Png);
            Image.Dispose();
        }
    }
}
