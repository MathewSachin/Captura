using System;
using System.Diagnostics;
using TestStack.White;
using TestStack.White.UIItems.WindowItems;

namespace Captura.Tests.Views
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AppRunnerFixture : IDisposable
    {
        public Application App { get; }
        public Window MainWindow { get; }

        public AppRunnerFixture()
        {
            App = Application.Launch(new ProcessStartInfo(TestManagerFixture.GetUiPath(), "--no-persist"));

            MainWindow = App.GetWindow(nameof(Captura));
        }

        public void Dispose()
        {
            MainWindow.Close();

            App.Close();
        }
    }
}