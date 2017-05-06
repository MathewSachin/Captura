using Screna;
using Screna.Audio;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Captura.Models
{
    /// <summary>
    /// Encode Video using FFMpeg.exe
    /// </summary>
    public class FFMpegMuxedWriter : IVideoFileWriter
    {
        static readonly string BaseDir = Path.Combine(Path.GetTempPath(), "Screna.FFMpeg");
        readonly AudioFileWriter _audioWriter;
        readonly FFMpegVideoWriter _videoWriter;
        readonly string _ffmpegArgs, tempVideoPath, tempAudioPath;

        /// <summary>
        /// Creates a new instance of <see cref="FFMpegMuxedWriter"/>.
        /// </summary>
        /// <param name="FilePath">Path for the output file.</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        public FFMpegMuxedWriter(string FilePath, int FrameRate, int VideoQuality, FFMpegVideoArgsProvider VideoArgsProvider, int AudioQuality, FFMpegAudioArgsProvider AudioArgsProvider, IAudioProvider AudioProvider)
        {
            if (AudioProvider == null)
                throw new ArgumentNullException(nameof(AudioProvider), $"{nameof(AudioProvider)} can't be null. Use {nameof(FFMpegVideoWriter)} instead.");

            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var fileName = Path.GetFileName(FilePath);
            tempVideoPath = Path.Combine(BaseDir, fileName);
            tempAudioPath = Path.Combine(BaseDir, Path.ChangeExtension(fileName, ".wav"));
            
            _audioWriter = new AudioFileWriter(tempAudioPath, AudioProvider.WaveFormat);

            _videoWriter = new FFMpegVideoWriter(tempVideoPath, FrameRate, VideoQuality, VideoArgsProvider);
            
            _ffmpegArgs = $"-i {tempVideoPath} -vcodec copy -i {tempAudioPath} {AudioArgsProvider(AudioQuality)} \"{FilePath}\"";
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _audioWriter.Dispose();
            _videoWriter.Dispose();

            var ffmpegProcess = new Process
            {
                StartInfo =
                {
                    FileName = "ffmpeg.exe",
                    Arguments = _ffmpegArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            ffmpegProcess.Start();
            ffmpegProcess.WaitForExit();
            
            File.Delete(tempAudioPath);
            File.Delete(tempVideoPath);
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
        public void WriteFrame(Bitmap Image) => _videoWriter.WriteFrame(Image);
    }
}
