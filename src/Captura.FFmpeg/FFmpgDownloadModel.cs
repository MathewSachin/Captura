using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Captura.Loc;

namespace Captura.FFmpeg
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegDownloadModel : NotifyPropertyChanged
    {
        readonly ProxySettings _proxySettings;
        readonly FFmpegSettings _ffmpegSettings;

        public FFmpegDownloadModel(ProxySettings ProxySettings,
            ILocalizationProvider Loc,
            FFmpegSettings FFmpegSettings)
        {
            _proxySettings = ProxySettings;

            _ffmpegSettings = FFmpegSettings;
        }

        public async Task<bool> Start(IProgress<FFmpegDownloaderProgress> Progress, CancellationToken CancellationToken)
        {
            // First progress report takes some time
            Progress.Report(new FFmpegDownloaderProgress(0));

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
                }, _proxySettings.GetWebProxy(), CancellationToken);
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

            // Download complete
            Progress.Report(new FFmpegDownloaderProgress(100));

            Progress.Report(new FFmpegDownloaderProgress(FFmpegDownloaderState.Extracting));

            try
            {
                var ffmpegFolder = _ffmpegSettings.GetFolderPath();

                if (!Directory.Exists(ffmpegFolder))
                {
                    Directory.CreateDirectory(ffmpegFolder);
                }

                await DownloadFFmpeg.ExtractTo(ffmpegFolder);
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
