using System;
using System.Net;
using System.Threading.Tasks;
using Squirrel;

namespace Captura
{
    class ProxiedDownloader : IFileDownloader
    {
        public Task DownloadFile(string Url, string TargetFile, Action<int> Progress)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var web = new WebClient { Proxy = Settings.Instance.GetWebProxy() })
                {
                    web.DownloadProgressChanged += (s, e) => Progress?.Invoke(e.ProgressPercentage);

                    web.DownloadFile(Url, TargetFile);
                }
            });
        }

        public Task<byte[]> DownloadUrl(string Url)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var web = new WebClient { Proxy = Settings.Instance.GetWebProxy() })
                {
                    return web.DownloadData(Url);
                }
            });
        }
    }
}