using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UpdateChecker : IUpdateChecker
    {
        readonly ProxySettings _proxySettings;
        readonly Version _currentVersion;

        public UpdateChecker(ProxySettings ProxySettings)
        {
            _proxySettings = ProxySettings;

            _currentVersion = ServiceProvider.AppVersion;
        }

        public void GoToDownloadsPage()
        {
            Process.Start(DownloadsUrl);
        }

        const string DownloadsUrl = "https://mathewsachin.github.io/Captura/download";
        const string LatestReleaseUrl = "https://api.github.com/repos/MathewSachin/Captura/releases/latest";

        public async Task<Version> Check()
        {
            using (var w = new WebClient { Proxy = _proxySettings.GetWebProxy() })
            {
                // User Agent header required by GitHub api
                w.Headers.Add("user-agent", nameof(Captura));

                var result = await w.DownloadStringTaskAsync(LatestReleaseUrl);

                var jObj = JObject.Parse(result);

                // tag_name = v0.0.0 for stable releases
                var version = Version.Parse(jObj["tag_name"].ToString().Substring(1));

                if (version > _currentVersion)
                {
                    return version;
                }
            }

            return null;
        }

        public string BuildName => "STABLE";
    }
}