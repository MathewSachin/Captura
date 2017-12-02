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

        WebClient web;

        async Task Start()
        {
            if (ActionDescription == CancelDownload)
            {
                web.CancelAsync();
                
                return;
            }

            ActionDescription = CancelDownload;

            var archivePath = Path.Combine(Path.GetTempPath(), "ffmpeg.7z");

            using (web = new WebClient { Proxy = Settings.Instance.GetWebProxy() })
            {
                web.DownloadProgressChanged += (s, e) =>
                {
                    Progress = e.ProgressPercentage;

                    Status = $"Downloading ({Progress}%)";
                };

                Status = "Downloading";

                try
                {
                    await web.DownloadFileTaskAsync(FFMpegUri, archivePath);
                }
                catch (WebException e) when (e.Status == WebExceptionStatus.RequestCanceled)
                {
                    Status = "Cancelled";

                    return;
                }
                catch (Exception e)
                {
                    Status = $"Failed {e.Message}";

                    return;
                }
                finally
                {
                    // No cancelling after download
                    StartCommand.RaiseCanExecuteChanged(false);
                }
            }

            Status = "Extracting";

            try
            {
                await Task.Run(() =>
                {
                    using (var archive = ArchiveFactory.Open(archivePath))
                    {
                        // Find ffmpeg.exe
                        var ffmpegEntry = archive.Entries.First(entry => Path.GetFileName(entry.Key) == "ffmpeg.exe");

                        ffmpegEntry.WriteToDirectory(TargetFolder, new ExtractionOptions
                        {
                            // Don't copy directory structure
                            ExtractFullPath = false,
                            Overwrite = true
                        });
                    }
                });
            }
            catch (UnauthorizedAccessException)
            {
                Status = "Can't extract to specified directory";

                return;
            }
            catch
            {
                Status = "Extraction Failed";

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
