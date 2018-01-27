using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Readers;

namespace Captura.ViewModels
{
    public static class DownloadFFMpeg
    {
        static readonly Uri FFMpegUri;
        static readonly string FFMpegArchivePath;

        static DownloadFFMpeg()
        {
            var bits = Environment.Is64BitOperatingSystem ? 64 : 32;

            FFMpegUri = new Uri($"http://ffmpeg.zeranoe.com/builds/win{bits}/static/ffmpeg-latest-win{bits}-static.7z");

            FFMpegArchivePath = Path.Combine(Path.GetTempPath(), "ffmpeg.7z");
        }

        public static async Task DownloadArchive(Action<int> Progress, WebProxy Proxy, CancellationToken CancellationToken)
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

        public static async Task ExtractTo(string FolderPath)
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
    }
}