using FirstFloor.ModernUI.Presentation;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Captura
{
    public partial class App
    {
        public static CmdOptions CmdOptions { get; } = new CmdOptions();

        void Application_Startup(object Sender, StartupEventArgs Args)
        {
            AppDomain.CurrentDomain.UnhandledException += (S, E) =>
            {
                var dir = Path.Combine(ServiceProvider.SettingsDir, "Crashes");

                Directory.CreateDirectory(dir);

                File.WriteAllText(Path.Combine(dir, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt"), E.ExceptionObject.ToString());

                MessageBox.Show($"Unexpected error occured. Captura will exit.\n\n{E.ExceptionObject}", "App Crash", MessageBoxButton.OK, MessageBoxImage.Error);

                Shutdown();
            };

            CommandLine.Parser.Default.ParseArguments(Args.Args, CmdOptions);

            if (CmdOptions.Settings != null)
            {
                ServiceProvider.SettingsDir = CmdOptions.Settings;
            }

            if (CmdOptions.Reset)
            {
                Settings.Instance.SafeReset();
            }

            if (Settings.Instance.DarkTheme)
            {
                AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
            }

            var accent = Settings.Instance.AccentColor;

            if (!string.IsNullOrEmpty(accent))
            {
                if (ColorConverter.ConvertFromString(accent) is Color accentColor)
                    AppearanceManager.Current.AccentColor = accentColor;
            }

            // A quick fix for WpfToolkit not being copied to build output of console project
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Xceed.Wpf.Toolkit.ColorSortingMode.Alphabetical.ToString();
        }
    }
}