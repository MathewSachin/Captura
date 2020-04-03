using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Xunit;

namespace Captura.Tests.Views
{
    [Collection(nameof(Tests))]
    public class ScreenShotTests : IClassFixture<AppRunnerFixture>
    {
        readonly AppRunnerFixture _appRunner;

        public ScreenShotTests(AppRunnerFixture AppRunner)
        {
            _appRunner = AppRunner;
        }

        static void Shot(string FileName, IntPtr Window)
        {
            Thread.Sleep(500);

            var startInfo = new ProcessStartInfo
            {
                FileName = TestManagerFixture.GetCliPath(),
                Arguments = $"shot --source win:{Window} -f {FileName}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            process?.WaitForExit();

            Assert.False(process == null || process.ExitCode != 0,
                $"Error occurred when taking ScreenShot, hWnd: {Window}, FileName: {FileName}, ExitCode: {process?.ExitCode}");

            Assert.True(File.Exists(FileName), $"ScreenShot was not saved: {FileName}");
        }

        /// <summary>
        /// Take ScreenShot of all Tabs.
        /// </summary>
        // TODO: Don't skip this test
        [Fact(Skip = "This is failing randomly on AppVeyor")]
        public void ScreenShotTabs()
        {
            Shot("shot.png", _appRunner.App.Process.MainWindowHandle);
        }
    }
}
