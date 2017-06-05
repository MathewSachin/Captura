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
        }
    }
}