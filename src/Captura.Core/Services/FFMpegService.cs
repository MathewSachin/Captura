using Captura.Models;
using Ookii.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Captura
{
    public static class FFMpegService
    {
        public const string FFMpegExeName = "ffmpeg.exe";

        static Settings GetSettings() => ServiceProvider.Get<Settings>();

        public static bool FFMpegExists
        {
            get
            {
                var settings = GetSettings();

                // FFMpeg folder
                if (!string.IsNullOrWhiteSpace(settings.FFMpegFolder))
                {
                    var path = Path.Combine(settings.FFMpegFolder, FFMpegExeName);

                    if (File.Exists(path))
                        return true;
                }

                // application directory
                var cpath = Path.Combine(Assembly.GetExecutingAssembly().Location, FFMpegExeName);

                if (File.Exists(cpath))
                    return true;

                // Current working directory
                if (File.Exists(FFMpegExeName))
                    return true;

                // PATH
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = FFMpegExeName,
                        Arguments = "-version",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    return true;
                }
                catch { return false; }
            }
        }

        public static string FFMpegExePath
        {
            get
            {
                var settings = GetSettings();

                // FFMpeg folder
                if (!string.IsNullOrWhiteSpace(settings.FFMpegFolder))
                {
                    var path = Path.Combine(settings.FFMpegFolder, FFMpegExeName);

                    if (File.Exists(path))
                        return path;
                }

                // application directory
                var cpath = Path.Combine(Assembly.GetExecutingAssembly().Location, FFMpegExeName);

                return File.Exists(cpath) ? cpath : FFMpegExeName;
            }
        }

        public static void SelectFFMpegFolder()
        {
            var settings = GetSettings();

            using (var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = settings.FFMpegFolder,
                UseDescriptionForTitle = true,
                Description = LanguageManager.Instance.SelectFFMpegFolder
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    settings.FFMpegFolder = dlg.SelectedPath;
            }
        }

        public static Action FFMpegDownloader { get; set; }

        public static Process StartFFMpeg(string Arguments)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = FFMpegExePath,
                    Arguments = Arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true
                },
                EnableRaisingEvents = true
            };
                        
            process.ErrorDataReceived += (s, e) => FFMpegLog.Instance.Write(e.Data);

            process.Start();

            process.BeginErrorReadLine();
            
            return process;
        }
    }
}
