using SharpCompress.Archives;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Net;

namespace Captura.ViewModels
{
    public class FFMpgDownloadViewModel : ViewModelBase
    {
        public async void Start()
        {
            using (var web = new WebClient())
            {
                web.DownloadProgressChanged += (s, e) => Progress = e.ProgressPercentage;

                var bits = Environment.Is64BitOperatingSystem ? 64 : 32;
                
                var url = $"http://ffmpeg.zeranoe.com/builds/win{bits}/static/ffmpeg-latest-win{bits}-static.7z";

                var archivePath = Path.Combine(Path.GetTempPath(), "ffmpeg.7z");

                await web.DownloadFileTaskAsync(new Uri(url), archivePath);

                using (var archive = ArchiveFactory.Open(archivePath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (Path.GetFileName(entry.Key) == "ffmpeg.exe")
                        {
                            var mydocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                            entry.WriteToDirectory(mydocs, new ExtractionOptions() { ExtractFullPath = false, Overwrite = true });

                            Settings.Instance.FFMpegFolder = mydocs;
                        }
                    }
                }
            }
        }

        int _progress = 0;

        public int Progress
        {
            get => _progress;
            private set
            {
                _progress = value;

                OnPropertyChanged();
            }
        }

        string _status;

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
