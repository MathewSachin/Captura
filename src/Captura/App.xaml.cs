using FirstFloor.ModernUI.Presentation;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Captura.Views;
using CommandLine;

namespace Captura
{
    public partial class App
    {
        public static CmdOptions CmdOptions { get; private set; }
        
        void App_OnDispatcherUnhandledException(object Sender, DispatcherUnhandledExceptionEventArgs Args)
        {
            var dir = Path.Combine(ServiceProvider.SettingsDir, "Crashes");

            Directory.CreateDirectory(dir);

            File.WriteAllText(Path.Combine(dir, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt"), Args.Exception.ToString());

            Args.Handled = true;

            new ExceptionWindow(Args.Exception).ShowDialog();
        }

        void Application_Startup(object Sender, StartupEventArgs Args)
        {
            AppDomain.CurrentDomain.UnhandledException += (S, E) =>
            {
                var dir = Path.Combine(ServiceProvider.SettingsDir, "Crashes");

                Directory.CreateDirectory(dir);

                File.WriteAllText(Path.Combine(dir, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt"), E.ExceptionObject.ToString());

                if (E.ExceptionObject is Exception e)
                {
                    Current.Dispatcher.Invoke(() => new ExceptionWindow(e).ShowDialog());
                }

                Shutdown();
            };

            ServiceProvider.LoadModule(new CoreModule());

            Parser.Default.ParseArguments<CmdOptions>(Args.Args)
                .WithParsed(M => CmdOptions = M);

            if (CmdOptions.Settings != null)
            {
                ServiceProvider.SettingsDir = CmdOptions.Settings;
            }

            var settings = ServiceProvider.Get<Settings>();

            if (!CmdOptions.Reset)
            {
                settings.Load();
            }

            if (settings.UI.DarkTheme)
            {
                AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
            }

            var accent = settings.UI.AccentColor;

            if (!string.IsNullOrEmpty(accent))
            {
                AppearanceManager.Current.AccentColor = WpfExtensions.ParseColor(accent);
            }

            if (!string.IsNullOrWhiteSpace(settings.UI.Language))
            {
                var matchedCulture = LanguageManager.Instance.AvailableCultures.FirstOrDefault(M => M.Name == settings.UI.Language);

                if (matchedCulture != null)
                    LanguageManager.Instance.CurrentCulture = matchedCulture;
            }

            LanguageManager.Instance.LanguageChanged += L => settings.UI.Language = L.Name;
        }
    }
}