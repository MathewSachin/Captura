using Captura.Models;
using Ookii.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Captura
{
    public static class FFmpegService
    {
        public const string FFmpegExeName = "ffmpeg.exe";

        static Settings GetSettings() => ServiceProvider.Get<Settings>();

        public static bool FFmpegExists
        {
            get
            {
                var settings = GetSettings();

                // FFmpeg folder
                if (!string.IsNullOrWhiteSpace(settings.FFmpeg.FolderPath))
                {
                    var path = Path.Combine(settings.FFmpeg.FolderPath, FFmpegExeName);

                    if (File.Exists(path))
                        return true;
                }

                // application directory
                var cpath = Path.Combine(Assembly.GetExecutingAssembly().Location, FFmpegExeName);

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
                if (!string.IsNullOrWhiteSpace(settings.FFmpeg.FolderPath))
                {
                    var path = Path.Combine(settings.FFmpeg.FolderPath, FFmpegExeName);

                    if (File.Exists(path))
                        return path;
                }

                // application directory
                var cpath = Path.Combine(Assembly.GetExecutingAssembly().Location, FFmpegExeName);

                return File.Exists(cpath) ? cpath : FFmpegExeName;
            }
        }

        public static void SelectFFmpegFolder()
        {
            var settings = GetSettings();

            using (var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = settings.FFmpeg.FolderPath,
                UseDescriptionForTitle = true,
                Description = LanguageManager.Instance.SelectFFmpegFolder
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    settings.FFmpeg.FolderPath = dlg.SelectedPath;
            }
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

            var logItem = FFmpegLog.Instance.CreateNew(Path.GetFileName(OutputFileName));
                        
            process.ErrorDataReceived += (s, e) => logItem.Write(e.Data);

            process.Start();

            process.BeginErrorReadLine();
            
            return process;
        }
    }
}
