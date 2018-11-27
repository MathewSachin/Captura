using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UpdateChecker
    {
        readonly ProxySettings _proxySettings;

        public UpdateChecker(ProxySettings ProxySettings)
        {
            _proxySettings = ProxySettings;
        }

        public void GoToDownloadsPage()
        {
            const string downloadsUrl = "https://mathewsachin.github.io/Captura/download";

            Process.Start(downloadsUrl);
        }

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
                var newVersion = jObj["tag_name"].ToString();
                var version = Version.Parse(newVersion.Substring(1));

                var currentVersion = Assembly.GetEntryAssembly().GetName().Version;

                if (version > currentVersion)
                {
                    return version;
                }
            }

            return null;
        }
    }
}