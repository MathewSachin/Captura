using System;
using System.IO;
using System.Threading.Tasks;
using Captura.Models;

namespace Captura.FFmpeg
{
    public class FFmpegRollingWriter : IVideoFileWriter
    {
        readonly VideoWriterArgs _videoWriterArgs;
        readonly int _duration;
        readonly long _frameCount;
        long _currentFrame = -1;

        const int NoOfFiles = 3;
        int _currentFile = -1;

        static readonly TempFileVideoCodec TempVideoCodec = new TempFileVideoCodec();

        IVideoFileWriter _currentWriter;

        string GetFileName(int Index)
        {
            return $"{_videoWriterArgs.FileName}.{Index}.mp4";
        }

        public FFmpegRollingWriter(VideoWriterArgs VideoWriterArgs, int Duration)
        {
            _videoWriterArgs = VideoWriterArgs;
            _duration = Duration;
            _frameCount = _videoWriterArgs.FrameRate * Duration;
        }

        public void Dispose()
        {
            _prevWriterDisposeTask?.Wait();
            _currentWriter?.Dispose();

            var currentFile = GetFileName(_currentFile);
            var prevFile = GetFileName((_currentFile - 1 + NoOfFiles) % NoOfFiles);

            var currentDuration = _currentFrame * 1000 / _videoWriterArgs.FrameRate;
            var prevDuration = _duration * 1000 - currentDuration;

            // Prev file only
            if (currentDuration == 0 && File.Exists(prevFile))
            {
                File.Move(prevFile, _videoWriterArgs.FileName);
            }
            else if (prevDuration == 0 || !File.Exists(prevFile)) // Current file only
            {
                File.Move(currentFile, _videoWriterArgs.FileName);
            }
            else // Concat
            {
                var argsBuilder = new FFmpegArgsBuilder();

                argsBuilder.AddInputFile(prevFile)
                    .AddArg($"-ss {TimeSpan.FromMilliseconds(currentDuration):g}");

                argsBuilder.AddInputFile(currentFile);

                const string filter = "[0:v] [1:v] concat=n=2:v=1:a=0 [v]";

                argsBuilder.AddOutputFile(_videoWriterArgs.FileName)
                    .AddArg($"-filter_complex \"{filter}\"")
                    .AddArg($"-map \"[v]\"");

                var args = argsBuilder.GetArgs();

                var process = FFmpegService.StartFFmpeg(args, _videoWriterArgs.FileName);

                process.WaitForExit();
            }

            for (var i = 0; i < NoOfFiles; i++)
            {
                var filename = GetFileName(i);

                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }
        }

        Task _prevWriterDisposeTask;

        IVideoFileWriter GetWriter()
        {
            ++_currentFrame;
            _currentFrame %= _frameCount;

            if (_currentFrame == 0)
            {
                if (_currentWriter != null)
                {
                    _prevWriterDisposeTask?.Wait();

                    var prevWriter = _currentWriter;
                    _prevWriterDisposeTask = Task.Run(() => prevWriter.Dispose());

                    _currentWriter = null;
                }

                ++_currentFile;
                _currentFile %= NoOfFiles;
            }

            if (_currentWriter == null)
            {
                _currentWriter = TempVideoCodec.GetVideoFileWriter(new VideoWriterArgs
                {
                    FileName = GetFileName(_currentFile),
                    VideoQuality = _videoWriterArgs.VideoQuality,
                    FrameRate = _videoWriterArgs.FrameRate,
                    ImageProvider = _videoWriterArgs.ImageProvider
                });
            }

            return _currentWriter;
        }

        readonly object _syncLock = new object();

        public void WriteFrame(IBitmapFrame Image)
        {
            IVideoFileWriter writer;

            lock (_syncLock)
            {
                writer = GetWriter();
            }

            writer.WriteFrame(Image);
        }

        public bool SupportsAudio => false;

        public void WriteAudio(byte[] Buffer, int Length) { }
    }
}