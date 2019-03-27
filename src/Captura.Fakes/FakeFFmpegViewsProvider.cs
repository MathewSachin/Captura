using System;
using Captura.FFmpeg;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class FakeFFmpegViewsProvider : IFFmpegViewsProvider
    {
        public void ShowLogs()
        {
        }

        public void ShowUnavailable()
        {
            Console.Error.WriteLine("FFmpeg is not available.\nYou can install ffmpeg by calling: captura ffmpeg --install [path]");
        }

        public void ShowDownloader()
        {
        }

        public void PickFolder()
        {
        }
    }
}