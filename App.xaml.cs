using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Captura.Properties;
using FirstFloor.ModernUI.Presentation;
using ScreenWorks;

namespace Captura
{
    public partial class App : Application
    {
        public static bool IsLamePresent { get; private set; }
        
        public static bool IsDirectShowPresent { get; private set; }

        string Dir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

        void Application_Startup(object sender, StartupEventArgs e)
        {
            #region Settings
            AppearanceManager.Current.ThemeSource = Settings.Default.MUITheme == "light" ?
                AppearanceSettings.LightTheme.Source : AppearanceSettings.DarkTheme.Source;

            AppearanceManager.Current.AccentColor = Settings.Default.MUIThemeColor;

            VideoSettings.MinimizeOnStart = Settings.Default.MinimizeOnStart;
            #endregion

            string LamePath = Path.Combine(Dir, string.Format("lameenc{0}.dll", Environment.Is64BitProcess ? "64" : "32"));

            if (!File.Exists(LamePath)) IsLamePresent = false;
            else
            {
                SharpAviEncoder.SetLameLocation(LamePath);
                IsLamePresent = true;
            }

            IsDirectShowPresent = File.Exists(Path.Combine(Dir, "DirectShowLib-2005.dll"));

            App.Current.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show(args.Exception.Message);
                if (args.Exception.InnerException != null) MessageBox.Show(args.Exception.InnerException.Message);
                args.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var E = args.ExceptionObject as Exception;

                MessageBox.Show(E.Message);
                if (E.InnerException != null) MessageBox.Show(E.InnerException.Message);
                if (args.IsTerminating)
                {
                    MessageBox.Show("App will terminate");
                    Application.Current.Shutdown();
                }
            };
        }

        void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Default.MUITheme = AppearanceSettings.selectedTheme == AppearanceSettings.LightTheme ? "light" : "dark";

            Settings.Default.MUIThemeColor = AppearanceManager.Current.AccentColor;

            Settings.Default.MinimizeOnStart = VideoSettings.MinimizeOnStart;

            Settings.Default.Save();
        }
    }
}