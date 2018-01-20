using Ookii.Dialogs;
using SharpCompress.Archives;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Captura.ViewModels
{
    public class DownloadFFMpeg
    {
        static readonly Uri FFMpegUri;
        static readonly string FFMpegArchivePath;

        static DownloadFFMpeg()
        {
            var bits = Environment.Is64BitOperatingSystem ? 64 : 32;

            FFMpegUri = new Uri($"http://ffmpeg.zeranoe.com/builds/win{bits}/static/ffmpeg-latest-win{bits}-static.7z");

            FFMpegArchivePath = Path.Combine(Path.GetTempPath(), "ffmpeg.7z");
        }

        WebClient _webClient;

        public async Task DownloadArchive(Action<int> Progress)
        {
            using (_webClient = new WebClient { Proxy = Settings.Instance.GetWebProxy() })
            {
                _webClient.DownloadProgressChanged += (s, e) =>
                {
                    Progress?.Invoke(e.ProgressPercentage);
                };
                
                await _webClient.DownloadFileTaskAsync(FFMpegUri, FFMpegArchivePath);
            }
        }

        public async Task ExtractTo(string FolderPath)
        {
            await Task.Run(() =>
            {
                using (var archive = ArchiveFactory.Open(FFMpegArchivePath))
                {
                    // Find ffmpeg.exe
                    var ffmpegEntry = archive.Entries.First(Entry => Path.GetFileName(Entry.Key) == "ffmpeg.exe");

                    ffmpegEntry.WriteToDirectory(FolderPath, new ExtractionOptions
                    {
                        // Don't copy directory structure
                        ExtractFullPath = false,
                        Overwrite = true
                    });
                }
            });
        }
        
        public void Cancel()
        {
            _webClient?.CancelAsync();
        }
    }

    public class FFMpegDownloadViewModel : ViewModelBase
    {
        public DelegateCommand StartCommand { get; }

        public DelegateCommand SelectFolderCommand { get; }

        readonly DownloadFFMpeg _downloader = new DownloadFFMpeg();
        
        public FFMpegDownloadViewModel()
        {
            StartCommand = new DelegateCommand(async () => await Start());

            SelectFolderCommand = new DelegateCommand(() =>
            {
                using (var dlg = new VistaFolderBrowserDialog
                {
                    SelectedPath = TargetFolder,
                    UseDescriptionForTitle = true,
                    Description = LanguageManager.SelectFFMpegFolder
                })
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                        TargetFolder = dlg.SelectedPath;
                }
            });
        }

        const string CancelDownload = "Cancel Download";
        const string StartDownload = "Start Download";
        
        public async Task Start()
        {
            if (ActionDescription == CancelDownload)
            {
                _downloader.Cancel();
                
                return;
            }

            ActionDescription = CancelDownload;

            Status = "Downloading";

            try
            {
                await _downloader.DownloadArchive(P =>
                {
                    Progress = P;

                    Status = $"Downloading ({P}%)";
                });
            }
            catch (WebException webException) when(webException.Status == WebExceptionStatus.RequestCanceled)
            {
                Status = "Cancelled";
                return;
            }
            catch (Exception e)
            {
                Status = $"Failed - {e.Message}";
                return;
            }

            // No cancelling after download
            StartCommand.RaiseCanExecuteChanged(false);
            
            Status = "Extracting";

            try
            {
                await _downloader.ExtractTo(TargetFolder);
            }
            catch (UnauthorizedAccessException)
            {
                Status = "Can't extract to specified directory";
                return;
            }
            catch
            {
                Status = "Extraction failed";
                return;
            }
            
            // Update FFMpeg folder setting
            Settings.Instance.FFMpegFolder = TargetFolder;

            Status = "Done";
        }

        string _actionDescription = StartDownload;

        public string ActionDescription
        {
            get => _actionDescription;
            set
            {
                _actionDescription = value;

                OnPropertyChanged();
            }
        }

        string _targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string TargetFolder
        {
            get => _targetFolder;
            set
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
