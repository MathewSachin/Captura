using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using Captura.Properties;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.Finders;
using System.Diagnostics;
using TestStack.White.UIItems.WindowItems;
using System.Threading;
using System.IO;

namespace UITests
{
    [TestClass]
    public class ScreenShots
    {
        static Application App;
        static Window MainWindow;
        static TabPage MainTab;

        [ClassInitialize]
        public static void Init(TestContext Context)
        {
            App = Application.Launch("captura.ui.exe");

            MainWindow = App.GetWindow("Captura");

            MainTab = MainWindow.Get<TabPage>(SearchCriteria.ByText(Resources.Main));
        }

        [ClassCleanup]
        public static void Clean()
        {
            MainWindow.Close();

            App.Close();
        }

        static void Shot(string FileName, IntPtr hWnd)
        {
            Thread.Sleep(500);

            var startInfo = new ProcessStartInfo
            {
                FileName = "captura",
                Arguments = $"shot --source win:{hWnd} -f {FileName}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            process?.WaitForExit();

            if (process == null || process.ExitCode != 0)
                Assert.Fail($"Error occured when taking ScreenShot, hWnd: {hWnd}, FileName: {FileName}, ExitCode: {process.ExitCode}");
        }

        /// <summary>
        /// Take ScreenShot of all Tabs.
        /// </summary>
        [TestMethod]
        public void ScreenShotTabs()
        {
            Directory.CreateDirectory("Tabs");
            
            foreach (var tabName in new[] { Resources.Main, Resources.Configure, Resources.Recent, Resources.About })
            {
                var tab = MainWindow.Get<TabPage>(SearchCriteria.ByText(tabName));

                tab.Select();

                Shot($"Tabs/{tabName}.png", App.Process.MainWindowHandle);
            }
        }
    }
}
