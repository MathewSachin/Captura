using System.Windows;

namespace Captura
{
    public partial class App
    {
        public static CmdOptions CmdOptions { get; } = new CmdOptions();

        void Application_Startup(object sender, StartupEventArgs e)
        {
            CommandLine.Parser.Default.ParseArguments(e.Args, CmdOptions);

            if (CmdOptions.Reset)
            {
                Settings.Instance.Reset();
                Settings.Instance.UpdateRequired = false;
            }

            // A quick fix for MUI not being copied to build output of console project
            FirstFloor.ModernUI.Presentation.AppearanceManager.Current.ToString();

            // A quick fix for WpfToolkit not being copied to build output of console project
            Xceed.Wpf.Toolkit.ColorSortingMode.Alphabetical.ToString();
        }
    }
}