using System.Windows;
using System.Windows.Media;

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

            // A quick fix for MUI not being copied to bui;d output of console project
            FirstFloor.ModernUI.Presentation.AppearanceManager.Current.ToString();
        }
    }
}