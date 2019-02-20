﻿using Captura.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;

namespace Captura
{
    public static class FFmpegService
    {
        const string FFmpegExeName = "ffmpeg.exe";

        static FFmpegSettings GetSettings() => ServiceProvider.Get<FFmpegSettings>();

        public static bool FFmpegExists
        {
            get
            {
                var folderPath = GetSettings().GetFolderPath();

                // FFmpeg folder
                if (!string.IsNullOrWhiteSpace(folderPath))
                {
                    var path = Path.Combine(folderPath, FFmpegExeName);

                    if (File.Exists(path))
                        return true;
                }

                if (ServiceProvider.FileExists(FFmpegExeName))
                    return true;

                // PATH
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = FFmpegExeName,
                        Arguments = "-version",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    return true;
                }
                catch { return false; }
            }
        }

        public static string FFmpegExePath
        {
            get
            {
                var folderPath = GetSettings().GetFolderPath();

                // FFmpeg folder
                if (!string.IsNullOrWhiteSpace(folderPath))
                {
                    var path = Path.Combine(folderPath, FFmpegExeName);

                    if (File.Exists(path))
                        return path;
                }

                return new[] { ServiceProvider.AppDir, ServiceProvider.LibDir }
                           .Where(M => M != null)
                           .FirstOrDefault(M => File.Exists(Path.Combine(M, FFmpegExeName)))
                       ?? FFmpegExeName;
            }
        }

        public static void SelectFFmpegFolder()
        {
            var settings = GetSettings();

            var dialogService = ServiceProvider.Get<IDialogService>();

            var folder = dialogService.PickFolder(settings.GetFolderPath(), LanguageManager.Instance.SelectFFmpegFolder);
            
            if (!string.IsNullOrWhiteSpace(folder))
                settings.FolderPath = folder;
        }

        public static Action FFmpegDownloader { get; set; }

        public static Process StartFFmpeg(string Arguments, string OutputFileName)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = FFmpegExePath,
                    Arguments = Arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true
                },
                EnableRaisingEvents = true
            };

            var logItem = ServiceProvider.Get<FFmpegLog>().CreateNew(Path.GetFileName(OutputFileName));
                        
            process.ErrorDataReceived += (S, E) => logItem.Write(E.Data);

            process.Start();

            process.BeginErrorReadLine();
            
            return process;
        }

        public static bool WaitForConnection(this NamedPipeServerStream ServerStream, int Timeout)
        {
            var asyncResult = ServerStream.BeginWaitForConnection(Ar => {}, null);

            if (asyncResult.AsyncWaitHandle.WaitOne(Timeout))
            {
                ServerStream.EndWaitForConnection(asyncResult);

                return ServerStream.IsConnected;
            }

            return false;
        }
    }
}
