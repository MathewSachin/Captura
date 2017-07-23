using Captura.Models;
using Captura.ViewModels;
using Captura.Views;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Captura
{
    public partial class MainWindow
    {
        FFMpegDownloader _downloader;

        public MainWindow()
        {
            ServiceProvider.RegionProvider = new RegionSelector();

            ServiceProvider.MessageProvider = new MessageProvider();

            ServiceProvider.WebCamProvider = new WebCamProvider();

            FFMpegService.FFMpegDownloader += () =>
            {
                if (_downloader == null)
                {
                    _downloader = new FFMpegDownloader();
                    _downloader.Closed += (s, args) => _downloader = null;
                }

                _downloader.ShowAndFocus();
            };
            
            InitializeComponent();

            ServiceProvider.MainWindow = new MainWindowProvider(this);

            if (App.CmdOptions.Tray)
                Hide();
            
            // Register for Windows Messages
            ComponentDispatcher.ThreadPreprocessMessage += (ref MSG Message, ref bool Handled) =>
            {
                const int WmHotkey = 786;

                if (Message.message == WmHotkey)
                {
                    var id = Message.wParam.ToInt32();

                    ServiceProvider.RaiseHotKeyPressed(id);
                }
            };

            ServiceProvider.SystemTray = new SystemTray(SystemTray);
            
            Closing += (s, e) =>
            {
                if (!TryExit())
                    e.Cancel = true;
            };

            (DataContext as MainViewModel).Init(!App.CmdOptions.NoPersist, true, !App.CmdOptions.Reset, !App.CmdOptions.NoHotkeys);

            if (Settings.Instance.CheckForUpdates)
                CheckForUpdates();
        }

        async void CheckForUpdates()
        {
            try
            {
                var link = "https://api.github.com/repos/MathewSachin/Captura/releases/latest";

                string result;

                using (var web = new WebClient { Proxy = Settings.Instance.GetWebProxy() })
                {
                    web.Headers.Add(HttpRequestHeader.UserAgent, Properties.Resources.AppName);

                    result = await web.DownloadStringTaskAsync(link);
                }

                var node = JsonConvert.DeserializeXmlNode(result, "Releases");

                var latestVersion = node.SelectSingleNode("Releases/tag_name").InnerText.Substring(1);

                if (new Version(latestVersion) > AboutViewModel.Version)
                {
                    ServiceProvider.SystemTray.ShowTextNotification($"Captura: Update (v{latestVersion}) Available", 60_000, () =>
                    {
                        try
                        {
                            Process.Start("https://github.com/MathewSachin/Captura/releases/latest");
                        }
                        catch
                        {
                            // Swallow Exceptions.
                        }
                    });
                }
            }
            catch
            {
                // Swallow any Exceptions.
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized && Settings.Instance.MinimizeToTray)
                Hide();
        }

        void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

            e.Handled = true;
        }

        void MinButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        void SystemTray_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                Show();

                WindowState = WindowState.Normal;

                Activate();
            }
        }

        bool TryExit()
        {
            var vm = DataContext as MainViewModel;

            if (vm.RecorderState == RecorderState.Recording)
            {
                if (!ServiceProvider.MessageProvider.ShowYesNo("A Recording is in progress. Are you sure you want to exit?", "Confirm Exit"))
                    return false;
            }

            vm.Dispose();
            SystemTray.Dispose();
            Application.Current.Shutdown();

            return true;
        }

        void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            TryExit();
        }
    }
}
