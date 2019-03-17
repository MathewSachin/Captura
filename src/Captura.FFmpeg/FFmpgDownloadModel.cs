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

        FFmpegDownloaderState _downloaderState = FFmpegDownloaderState.Ready;

        public FFmpegDownloaderState State
        {
            get => _downloaderState;
            set => Set(ref _downloaderState, value);
        }

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
        
        public async Task<bool> Start(Action<int> Progress)
        {
            State = FFmpegDownloaderState.Downloading;

            try
            {
                await DownloadFFmpeg.DownloadArchive(P =>
                {
                    Progress?.Invoke(P);
                }, _proxySettings.GetWebProxy(), _cancellationTokenSource.Token);
            }
            catch (WebException webException) when(webException.Status == WebExceptionStatus.RequestCanceled)
            {
                State = FFmpegDownloaderState.Cancelled;
                return false;
            }
            catch (Exception e)
            {
                State = FFmpegDownloaderState.Error;
                Error = $"Failed - {e.Message}";
                return false;
            }

            _cancellationTokenSource.Dispose();

            State = FFmpegDownloaderState.Extracting;

            try
            {
                await DownloadFFmpeg.ExtractTo(FFmpegSettings.GetFolderPath());
            }
            catch (UnauthorizedAccessException)
            {
                State = FFmpegDownloaderState.Error;
                Error = "Can't extract to specified directory";
                return false;
            }
            catch
            {
                State = FFmpegDownloaderState.Error;
                Error = "Extraction failed";
                return false;
            }

            State = FFmpegDownloaderState.Done;

            return true;
        }

        string _error = "";

        public string Error
        {
            get => _error;
            private set => Set(ref _error, value);
        }
    }
}
