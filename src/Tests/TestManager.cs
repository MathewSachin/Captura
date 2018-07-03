using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captura.Tests
{
    [TestClass]
    public class TestManager
    {
        [AssemblyInitialize]
        public static void Init(TestContext Context)
        {
            ServiceProvider.LoadModule(new CoreModule());

            ServiceProvider.LoadModule(new FakesModule());
        }

        static string GetPath(string FolderName, string FileName)
        {
            var path = Assembly.GetExecutingAssembly().Location;

#if DEBUG
            const string config = "Debug";
#else
            const string config = "Release";
#endif

            for (var i = 0; i < 4; ++i)
                path = Path.GetDirectoryName(path);

            return Path.Combine(path, $"{FolderName}/bin/{config}/{FileName}");
        }

        public static string GetCliPath()
        {
            return GetPath("Captura.Console", "captura-cli.exe");
        }

        public static string GetUiPath()
        {
            return GetPath("Captura", "captura.exe");
        }
    }
}