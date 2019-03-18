using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Captura.Models;

namespace Captura.FFmpeg
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegDownloadModel : NotifyPropertyChanged
    {
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        readonly ProxySettings _proxySettings;
        readonly FFmpegSettings FFmpegSettings;

        public FFmpegDownloadModel(ProxySettings ProxySettings,
            ILocalizationProvider Loc,
            FFmpegSettings FFmpegSettings)
        {
            _proxySettings = ProxySettings;

            this.FFmpegSettings = FFmpegSettings;

            EnsureDir();
        }

        void EnsureDir()
        {
            var path = FFmpegSettings.GetFolderPath();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
        
        public async Task<bool> Start(IProgress<FFmpegDownloaderProgress> Progress)
        {
            try
            {
                var lastProgress = -1;

                await DownloadFFmpeg.DownloadArchive(P =>
                {
                    if (lastProgress == P)
                        return;

                    // Report only if changed
                    Progress.Report(new FFmpegDownloaderProgress(P));

                    lastProgress = P;
                }, _proxySettings.GetWebProxy(), _cancellationTokenSource.Token);
            }
            catch (WebException webException) when(webException.Status == WebExceptionStatus.RequestCanceled)
            {
                Progress.Report(new FFmpegDownloaderProgress(FFmpegDownloaderState.Cancelled));
                return false;
            }
            catch (Exception e)
            {
                Progress.Report(new FFmpegDownloaderProgress($"Failed - {e.Message}"));
                return false;
            }

            _cancellationTokenSource.Dispose();

            // Download complete
            Progress.Report(new FFmpegDownloaderProgress(100));

            Progress.Report(new FFmpegDownloaderProgress(FFmpegDownloaderState.Extracting));

            try
            {
                await DownloadFFmpeg.ExtractTo(FFmpegSettings.GetFolderPath());
            }
            catch (UnauthorizedAccessException)
            {
                Progress.Report(new FFmpegDownloaderProgress("Can't extract to specified directory"));
                return false;
            }
            catch
            {
                Progress.Report(new FFmpegDownloaderProgress("Extraction Failed"));
                return false;
            }

            Progress.Report(new FFmpegDownloaderProgress(FFmpegDownloaderState.Done));

            return true;
        }
    }
}
