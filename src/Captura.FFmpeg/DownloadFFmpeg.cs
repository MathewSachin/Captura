using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Captura.FFmpeg
{
    public static class DownloadFFmpeg
    {
        static readonly Uri FFmpegUri = new Uri("https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip");
        static readonly string FFmpegArchivePath;

        static DownloadFFmpeg()
        {
            FFmpegArchivePath = Path.Combine(Path.GetTempPath(), "ffmpeg.zip");
        }

        public static async Task DownloadArchive(Action<int> Progress, IWebProxy Proxy, CancellationToken CancellationToken)
        {
            if (!Environment.Is64BitOperatingSystem)
                throw new PlatformNotSupportedException("The FFmpeg download only provides 64-bit builds. Download a 32-bit build manually and set its folder in Captura.");

            using var webClient = new WebClient { Proxy = Proxy };
            CancellationToken.Register(() => webClient.CancelAsync());

            webClient.DownloadProgressChanged += (S, E) =>
            {
                Progress?.Invoke(E.ProgressPercentage);
            };
                
            await webClient.DownloadFileTaskAsync(FFmpegUri, FFmpegArchivePath);
        }

        const string ExeName = "ffmpeg.exe";

        public static async Task ExtractTo(string FolderPath)
        {
            await Task.Run(() =>
            {
                using var archive = ZipFile.OpenRead(FFmpegArchivePath);
                var ffmpegEntry = archive.Entries.First(M => M.Name == ExeName);

                ffmpegEntry.ExtractToFile(Path.Combine(FolderPath, ExeName), true);
            });
        }
    }
}