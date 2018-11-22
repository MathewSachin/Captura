using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace Captura.ViewModels
{
    // ReSharper disable once UnusedMember.Global
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

        async void Check()
        {
            if (Checking)
                return;

            Checking = true;
            UpdateAvailable = false;

            try
            {
                using (var w = new WebClient {Proxy = _settings.Proxy.GetWebProxy()})
                {
                    const string latestReleaseUrl = "https://api.github.com/repos/MathewSachin/Captura/releases/latest";

                    var result = await w.DownloadStringTaskAsync(latestReleaseUrl);

                    var jObj = JObject.Parse(result);

                    // tag_name = v0.0.0 for stable releases
                    var version = Version.Parse(jObj["tag_name"].ToString().Substring(1));

                    var currentVersion = Assembly.GetEntryAssembly().GetName().Version;

                    if (version > currentVersion)
                    {
                        UpdateAvailable = true;
                    }
                }
            }
            catch
            {
                CheckFailed = true;
            }
            finally
            {
                Checking = false;
            }
        }

        bool _checking, _available, _checkFailed;

        public bool Checking
        {
            get => _checking;
            set
            {
                _checking = value;

                OnPropertyChanged();
            }
        }

        public bool UpdateAvailable
        {
            get => _available;
            set
            {
                _available = value;

                OnPropertyChanged();
            }
        }

        public bool CheckFailed
        {
            get => _checkFailed;
            set
            {
                _checkFailed = value;

                OnPropertyChanged();
            }
        }

        public ICommand CheckCommand { get; }

        public ICommand GoToDownload { get; }
    }
}