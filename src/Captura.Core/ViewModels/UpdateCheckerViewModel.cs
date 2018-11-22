using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UpdateCheckerViewModel : NotifyPropertyChanged
    {
        readonly Settings _settings;

        public UpdateCheckerViewModel(Settings Settings)
        {
            _settings = Settings;
            Check();

            CheckCommand = new DelegateCommand(Check);

            GoToDownload = new DelegateCommand(() =>
            {
                const string downloadsUrl = "https://mathewsachin.github.io/Captura/download";

                Process.Start(downloadsUrl);
            });
        }

        const string LatestReleaseUrl = "https://api.github.com/repos/MathewSachin/Captura/releases/latest";

        void Check()
        {
            if (Checking)
                return;

            Checking = true;
            UpdateAvailable = false;
            CheckFailed = false;

            Task.Run(async () =>
            {
                try
                {
                    using (var w = new WebClient { Proxy = _settings.Proxy.GetWebProxy() })
                    {
                        // User Agent header required by GitHub api
                        w.Headers.Add("user-agent", nameof(Captura));

                        var result = await w.DownloadStringTaskAsync(LatestReleaseUrl);

                        var jObj = JObject.Parse(result);

                        // tag_name = v0.0.0 for stable releases
                        NewVersion = jObj["tag_name"].ToString();
                        var version = Version.Parse(NewVersion.Substring(1));

                        var currentVersion = Assembly.GetEntryAssembly().GetName().Version;

                        if (version > currentVersion)
                        {
                            UpdateAvailable = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                    CheckFailed = true;
                }
                finally
                {
                    Checking = false;
                }
            });
        }

        bool _checking, _available, _checkFailed;

        public bool Checking
        {
            get => _checking;
            private set
            {
                _checking = value;

                OnPropertyChanged();
            }
        }

        public bool UpdateAvailable
        {
            get => _available;
            private set
            {
                _available = value;

                OnPropertyChanged();
            }
        }

        public bool CheckFailed
        {
            get => _checkFailed;
            private set
            {
                _checkFailed = value;

                OnPropertyChanged();
            }
        }

        string _newVersion;

        public string NewVersion
        {
            get => _newVersion;
            private set
            {
                _newVersion = value;
                OnPropertyChanged();
            }
        }

        public ICommand CheckCommand { get; }

        public ICommand GoToDownload { get; }
    }
}