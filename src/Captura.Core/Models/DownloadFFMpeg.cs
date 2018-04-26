using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Captura.ViewModels
{
    public static class DownloadFFMpeg
    {
        static readonly Uri FFMpegUri;
        static readonly string FFMpegArchivePath;

        static DownloadFFMpeg()
        {
            var bits = Environment.Is64BitOperatingSystem ? 64 : 32;

            FFMpegUri = new Uri($"https://ffmpeg.zeranoe.com/builds/win{bits}/static/ffmpeg-latest-win{bits}-static.zip");

            FFMpegArchivePath = Path.Combine(Path.GetTempPath(), "ffmpeg.zip");
        }

        public static async Task DownloadArchive(Action<int> Progress, IWebProxy Proxy, CancellationToken CancellationToken)
        {
            using (var webClient = new WebClient { Proxy = Proxy })
            {
                CancellationToken.Register(() => webClient.CancelAsync());

                webClient.DownloadProgressChanged += (s, e) =>
                {
                    Progress?.Invoke(e.ProgressPercentage);
                };
                
                await webClient.DownloadFileTaskAsync(FFMpegUri, FFMpegArchivePath);
            }
        }

        const string ExeName = "ffmpeg.exe";

        public static async Task ExtractTo(string FolderPath)
        {
            await Task.Run(() =>
            {
                using (var archive = ZipFile.OpenRead(FFMpegArchivePath))
                {
                    var ffmpegEntry = archive.Entries.First(M => M.Name == ExeName);

                    ffmpegEntry.ExtractToFile(Path.Combine(FolderPath, ExeName), true);
                }
            });
        }
    }
}