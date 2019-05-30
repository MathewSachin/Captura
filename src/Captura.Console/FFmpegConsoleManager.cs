using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Captura.FFmpeg;

namespace Captura
{
    class FFmpegConsoleManager
    {
        readonly FFmpegDownloadModel _downloadModel;
        readonly FFmpegSettings _ffmpegSettings;
        readonly object _syncLock = new object();

        public FFmpegConsoleManager(FFmpegDownloadModel DownloadModel,
            FFmpegSettings FfmpegSettings)
        {
            _downloadModel = DownloadModel;
            _ffmpegSettings = FfmpegSettings;
        }

        public async Task Run(FFmpegCmdOptions FFmpegOptions)
        {
            if (FFmpegOptions.Install != null)
            {
                var downloadFolder = FFmpegOptions.Install;

                if (!Directory.Exists(downloadFolder))
                {
                    Directory.CreateDirectory(downloadFolder);
                }

                _ffmpegSettings.FolderPath = downloadFolder;

                var progress = new Progress<FFmpegDownloaderProgress>(FFmpegProgressHandler);

                Console.Write(nameof(FFmpegDownloaderState.Ready));

                var cts = new CancellationTokenSource();

                Console.CancelKeyPress += (S, E) =>
                {
                    cts.Cancel();

                    // Prevent abrupt exit
                    E.Cancel = true;
                };

                await _downloadModel.Start(progress, cts.Token);
            }
        }

        void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        void FFmpegProgressHandler(FFmpegDownloaderProgress DownloaderProgress)
        {
            // We don't want to write progress to files
            if (Console.IsOutputRedirected)
                return;

            // Locking is necessary to prevent weird output
            lock (_syncLock)
            {
                // Don't use WriteLine() in this block, only Write()
                ClearLastLine();

                switch (DownloaderProgress.State)
                {
                    case FFmpegDownloaderState.Error:
                        Console.Write(DownloaderProgress.ErrorMessage);
                        break;

                    case FFmpegDownloaderState.Downloading:
                        Console.Write($"{nameof(FFmpegDownloaderState.Downloading)} ({DownloaderProgress.DownloadProgress}%)");
                        break;

                    default:
                        Console.Write(DownloaderProgress.State);
                        break;
                }
            }
        }
    }
}