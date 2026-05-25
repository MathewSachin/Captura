using System;
using System.Diagnostics;
using System.Threading;

namespace Captura.Tests.Views
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AppRunnerFixture : IDisposable
    {
        public Process App { get; }

        public IntPtr MainWindowHandle => App.MainWindowHandle;

        public AppRunnerFixture()
        {
            App = Process.Start(new ProcessStartInfo(TestManagerFixture.GetUiPath(), "--no-persist")
            {
                UseShellExecute = false
            });

            if (App == null)
                throw new InvalidOperationException("Failed to start Captura UI process.");

            for (var i = 0; i < 20 && App.MainWindowHandle == IntPtr.Zero && !App.HasExited; i++)
            {
                Thread.Sleep(500);
                App.Refresh();
            }
        }

        public void Dispose()
        {
            if (App.HasExited)
                return;

            App.CloseMainWindow();

            if (!App.WaitForExit(5000))
                App.Kill();
        }
    }
}
