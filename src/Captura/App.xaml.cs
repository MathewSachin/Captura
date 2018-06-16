using FirstFloor.ModernUI.Presentation;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
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
                    new ExceptionWindow(e).ShowDialog();
                }

                Shutdown();
            };

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
                if (ColorConverter.ConvertFromString(accent) is Color accentColor)
                    AppearanceManager.Current.AccentColor = accentColor;
            }

            // A quick fix for WpfToolkit not being copied to build output of console project
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Xceed.Wpf.Toolkit.ColorSortingMode.Alphabetical.ToString();
        }
    }
}