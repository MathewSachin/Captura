using Captura.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;

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
                var settings = GetSettings();

                // FFmpeg folder
                if (!string.IsNullOrWhiteSpace(settings.FolderPath))
                {
                    var path = Path.Combine(settings.FolderPath, FFmpegExeName);

                    if (File.Exists(path))
                        return true;
                }

                // application directory
                var cpath = Path.Combine(Assembly.GetEntryAssembly().Location, FFmpegExeName);

                if (File.Exists(cpath))
                    return true;

                // Current working directory
                if (File.Exists(FFmpegExeName))
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
                var settings = GetSettings();

                // FFmpeg folder
                if (!string.IsNullOrWhiteSpace(settings.FolderPath))
                {
                    var path = Path.Combine(settings.FolderPath, FFmpegExeName);

                    if (File.Exists(path))
                        return path;
                }

                // application directory
                var cpath = Path.Combine(Assembly.GetEntryAssembly().Location, FFmpegExeName);

                return File.Exists(cpath) ? cpath : FFmpegExeName;
            }
        }

        public static void SelectFFmpegFolder()
        {
            var settings = GetSettings();

            var dialogService = ServiceProvider.Get<IDialogService>();

            var folder = dialogService.PickFolder(settings.FolderPath, LanguageManager.Instance.SelectFFmpegFolder);
            
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
