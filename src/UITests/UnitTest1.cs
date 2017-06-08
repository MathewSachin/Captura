using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using Captura.Properties;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.Finders;
using System.Diagnostics;
using TestStack.White.UIItems.WindowItems;
using System.Threading;

namespace UITests
{
    [TestClass]
    public class UnitTest1
    {
        Application App;

        Window MainWindow;

        void Shot()
        {
            Thread.Sleep(500);

            var startInfo = new ProcessStartInfo
            {
                FileName = "captura",
                Arguments = $"shot --source win:{App.Process.MainWindowHandle}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(startInfo).WaitForExit();
        }

        [TestMethod]
        public void TestMethod1()
        {
            App = Application.Launch("captura.ui.exe");

            MainWindow = App.GetWindow("Captura");
            
            // ScreenShot of all tabs            
            foreach (var tabName in new[] { Resources.Main, Resources.Configure, Resources.Recent, Resources.Hotkeys, Resources.About })
            {
                var tab = MainWindow.Get<TabPage>(SearchCriteria.ByText(tabName));

                tab.Select();

                Shot();
            }

            MainWindow.Close();
        }
    }
}
