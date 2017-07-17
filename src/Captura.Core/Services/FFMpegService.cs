using Captura.Properties;
using Ookii.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Captura
{
    public static class FFMpegService
    {
        public static bool FFMpegExists
        {
            get
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = FFMpegExePath,
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
                if (string.IsNullOrWhiteSpace(Settings.Instance.FFMpegFolder))
                    return "ffmpeg.exe";

                return Path.Combine(Settings.Instance.FFMpegFolder, "ffmpeg.exe");
            }
        }

        public static void SelectFFMpegFolder()
        {
            using (var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = Settings.Instance.FFMpegFolder,
                UseDescriptionForTitle = true,
                Description = Resources.SelectFFMpegFolder
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    Settings.Instance.FFMpegFolder = dlg.SelectedPath;
            }
        }

        public static Action FFMpegDownloader { get; set; }
    }
}
