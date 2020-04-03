using System;
using System.IO;
using Captura.Fakes;
using Captura.Video;

namespace Captura.Tests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TestManagerFixture : IDisposable
    {
        public TestManagerFixture()
        {
            ServiceProvider.LoadModule(new CoreModule());

            ServiceProvider.LoadModule(new FakesModule());
        }

        public void Dispose() { }

        static string GetPath(string FolderName, string FileName)
        {
            var path = typeof(IOverlay).Assembly.CodeBase
                .Split(new [] {"///"}, StringSplitOptions.None)[1];

#if DEBUG
            const string config = "Debug";
#else
            const string config = "Release";
#endif

            for (var i = 0; i < 5; ++i)
                path = Path.GetDirectoryName(path);

            return Path.Combine(path, $"{FolderName}/bin/{config}/{FileName}");
        }

        public static string GetCliPath()
        {
            return GetPath("Captura.Console", "captura-cli.exe");
        }

        public static string GetUiPath()
        {
            return GetPath(nameof(Captura), "captura.exe");
        }
    }
}