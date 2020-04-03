using System;
using System.IO;
using System.Threading.Tasks;
using Captura.Video;

namespace Captura.FFmpeg
{
    public class FFmpegReplayWriter : IVideoFileWriter
    {
        readonly VideoWriterArgs _videoWriterArgs;
        readonly int _duration;
        readonly long _frameCount;
        long _currentFrame = -1;

        const int NoOfFiles = 2;
        int _currentFile = -1;

        readonly Func<VideoWriterArgs, IVideoFileWriter> _writerGenerator;

        IVideoFileWriter _currentWriter;

        string GetFileName(int Index)
        {
            return $"{_videoWriterArgs.FileName}.{Index}.mp4";
        }

        public FFmpegReplayWriter(VideoWriterArgs VideoWriterArgs,
            int Duration,
            Func<VideoWriterArgs, IVideoFileWriter> WriterGenerator)
        {
            _videoWriterArgs = VideoWriterArgs;
            _duration = Duration;
            _writerGenerator = WriterGenerator;
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
                    .AddArg("ss", $"{TimeSpan.FromMilliseconds(currentDuration):g}");

                argsBuilder.AddInputFile(currentFile);

                var hasAudio = _videoWriterArgs.AudioProvider != null;

                const string filter = "[0:v] [1:v] concat=n=2:v=1:a=0 [v]";
                const string filterWithAudio = "[0:v] [0:a] [1:v] [1:a] concat=n=2:v=1:a=1 [v] [a]";

                var output = argsBuilder.AddOutputFile(_videoWriterArgs.FileName)
                    .AddArg($"-filter_complex \"{(hasAudio ? filterWithAudio : filter)}\"")
                    .AddArg("-map \"[v]\"");

                if (hasAudio)
                    output.AddArg("-map \"[a]\"");

                var args = argsBuilder.GetArgs();

                var process = FFmpegService.StartFFmpeg(args, _videoWriterArgs.FileName, out _);

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

            return _currentWriter ??= _writerGenerator(new VideoWriterArgs
            {
                FileName = GetFileName(_currentFile),
                VideoQuality = _videoWriterArgs.VideoQuality,
                FrameRate = _videoWriterArgs.FrameRate,
                ImageProvider = _videoWriterArgs.ImageProvider,
                AudioProvider = _videoWriterArgs.AudioProvider,
                AudioQuality = _videoWriterArgs.AudioQuality
            });
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

        public bool SupportsAudio => true;

        public void WriteAudio(byte[] Buffer, int Offset, int Length)
        {
            IVideoFileWriter writer;

            lock (_syncLock)
            {
                writer = _currentWriter;
            }

            writer?.WriteAudio(Buffer, Offset, Length);
        }
    }
}