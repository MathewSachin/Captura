using FirstFloor.ModernUI.Presentation;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Captura.Loc;
using Captura.Models;
using Captura.MouseKeyHook;
using Captura.ViewModels;
using Captura.Views;
using CommandLine;

namespace Captura
{
    public partial class App
    {
        public App()
        {
            SingleInstanceManager.SingleInstanceCheck();

            // Splash Screen should be created manually and after single-instance is checked
            ShowSplashScreen();
        }

        public static CmdOptions CmdOptions { get; private set; }
        
        void App_OnDispatcherUnhandledException(object Sender, DispatcherUnhandledExceptionEventArgs Args)
        {
            var dir = Path.Combine(ServiceProvider.SettingsDir, "Crashes");

            Directory.CreateDirectory(dir);

            File.WriteAllText(Path.Combine(dir, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt"), Args.Exception.ToString());

            Args.Handled = true;

            new ErrorWindow(Args.Exception, Args.Exception.Message).ShowDialog();
        }

        void ShowSplashScreen()
        {
            var splashScreen = new SplashScreen("Images/Logo.png");
            splashScreen.Show(true);
        }

        void Application_Startup(object Sender, StartupEventArgs Args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainOnUnhandledException;

            ServiceProvider.LoadModule(new CoreModule());
            ServiceProvider.LoadModule(new ViewCoreModule());

            Parser.Default.ParseArguments<CmdOptions>(Args.Args)
                .WithParsed(M => CmdOptions = M);

            if (CmdOptions.Settings != null)
            {
                ServiceProvider.SettingsDir = CmdOptions.Settings;
            }

            var settings = ServiceProvider.Get<Settings>();

            InitTheme(settings);

            BindLanguageSetting(settings);

            BindKeymapSetting(settings);
        }

        void OnCurrentDomainOnUnhandledException(object S, UnhandledExceptionEventArgs E)
        {
            var dir = Path.Combine(ServiceProvider.SettingsDir, "Crashes");

            Directory.CreateDirectory(dir);

            File.WriteAllText(Path.Combine(dir, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt"), E.ExceptionObject.ToString());

            if (E.ExceptionObject is Exception e)
            {
                Current.Dispatcher.Invoke(() => new ErrorWindow(e, e.Message).ShowDialog());
            }

            Shutdown();
        }

        static void BindKeymapSetting(Settings Settings)
        {
            var keymap = ServiceProvider.Get<KeymapViewModel>();

            if (!string.IsNullOrWhiteSpace(Settings.Keystrokes.KeymapName))
            {
                var matched = keymap.AvailableKeymaps.FirstOrDefault(M => M.Name == Settings.Keystrokes.KeymapName);

                if (matched != null)
                    keymap.SelectedKeymap = matched;
            }

            keymap.PropertyChanged += (S, E) => Settings.Keystrokes.KeymapName = keymap.SelectedKeymap.Name;
        }

        static void BindLanguageSetting(Settings Settings)
        {
            var loc = LanguageManager.Instance;

            if (!string.IsNullOrWhiteSpace(Settings.UI.Language))
            {
                var matchedCulture = loc.AvailableCultures.FirstOrDefault(M => M.Name == Settings.UI.Language);

                if (matchedCulture != null)
                    loc.CurrentCulture = matchedCulture;
            }

            loc.LanguageChanged += L => Settings.UI.Language = L.Name;
        }

        static void InitTheme(Settings Settings)
        {
            if (!CmdOptions.Reset)
            {
                Settings.Load();
            }

            if (Settings.UI.DarkTheme)
            {
                AppearanceManager.Current.ThemeSource = AppearanceManager.DarkThemeSource;
            }

            var accent = Settings.UI.AccentColor;

            if (!string.IsNullOrEmpty(accent))
            {
                AppearanceManager.Current.AccentColor = WpfExtensions.ParseColor(accent);
            }
        }
    }
}