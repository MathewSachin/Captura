using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DevUpdateChecker : IUpdateChecker
    {
        readonly ProxySettings _proxySettings;
        readonly Version _currentVersion;

        public DevUpdateChecker(ProxySettings ProxySettings)
        {
            _proxySettings = ProxySettings;

            _currentVersion = ServiceProvider.AppVersion;
        }

        public void GoToDownloadsPage()
        {
            Process.Start(DownloadsUrl);
        }

        const string DownloadsUrl = "https://ci.appveyor.com/project/MathewSachin/captura/branch/master";
        const string MasterBuildUrl = "https://ci.appveyor.com/api/projects/MathewSachin/Captura/branch/master";

        public async Task<Version> Check()
        {
            using (var w = new WebClient { Proxy = _proxySettings.GetWebProxy() })
            {
                var result = await w.DownloadStringTaskAsync(MasterBuildUrl);

                var jObj = JObject.Parse(result);

                var version = Version.Parse(jObj["build"]["version"].ToString());

                if (version > _currentVersion)
                {
                    return version;
                }
            }

            return null;
        }

        public string BuildName => _currentVersion.Build == 0 ? "DEV" : "CI";
    }
}