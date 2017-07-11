using Captura.Properties;
using Ookii.Dialogs;
using SharpCompress.Archives;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Captura.ViewModels
{
    public class FFMpegDownloadViewModel : ViewModelBase
    {
        public DelegateCommand StartCommand { get; }

        public DelegateCommand SelectFolderCommand { get; }

        static readonly Uri FFMpegUri;

        static FFMpegDownloadViewModel()
        {
            var bits = Environment.Is64BitOperatingSystem ? 64 : 32;

            FFMpegUri = new Uri($"http://ffmpeg.zeranoe.com/builds/win{bits}/static/ffmpeg-latest-win{bits}-static.7z");
        }

        public FFMpegDownloadViewModel()
        {
            StartCommand = new DelegateCommand(async () => await Start());

            SelectFolderCommand = new DelegateCommand(() =>
            {
                using (var dlg = new VistaFolderBrowserDialog
                {
                    SelectedPath = TargetFolder,
                    UseDescriptionForTitle = true,
                    Description = Resources.SelectFFMpegFolder
                })
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                        TargetFolder = dlg.SelectedPath;
                }
            });
        }

        async Task Start()
        {
            using (var web = new WebClient())
            {
                web.DownloadProgressChanged += (s, e) => Progress = e.ProgressPercentage;
                
                var archivePath = Path.Combine(Path.GetTempPath(), "ffmpeg.7z");

                await web.DownloadFileTaskAsync(FFMpegUri, archivePath);

                using (var archive = ArchiveFactory.Open(archivePath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (Path.GetFileName(entry.Key) == "ffmpeg.exe")
                        {
                            entry.WriteToDirectory(TargetFolder, new ExtractionOptions { ExtractFullPath = false, Overwrite = true });

                            Settings.Instance.FFMpegFolder = TargetFolder;
                        }
                    }
                }
            }
        }

        string _targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string TargetFolder
        {
            get => _targetFolder;
            private set
            {
                _targetFolder = value;

                OnPropertyChanged();
            }
        }

        int _progress;

        public int Progress
        {
            get => _progress;
            private set
            {
                _progress = value;

                OnPropertyChanged();
            }
        }

        string _status = "Ready";

        public string Status
        {
            get => _status;
            set
            {
                _status = value;

                OnPropertyChanged();
            }
        }
    }
}
