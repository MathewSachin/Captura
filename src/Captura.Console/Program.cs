using System;
using System.Diagnostics;
using System.IO;
using Captura.Models;
using Captura.Native;
using CommandLine;
using static System.Console;
// ReSharper disable LocalizableElement

namespace Captura
{
    static class Program
    {
        static void Main(string[] Args)
        {
            User32.SetProcessDPIAware();

            ServiceProvider.LoadModule(new CoreModule());
            ServiceProvider.LoadModule(new FakesModule());

            Parser.Default.ParseArguments<StartCmdOptions, ShotCmdOptions, FFmpegCmdOptions, ListCmdOptions, UploadCmdOptions>(Args)
                .WithParsed((ICmdlineVerb Verb) =>
                {
                    // Always display Banner
                    Banner();

                    Verb.Run();
                });
        }

        static void Banner()
        {
            var version = ServiceProvider.AppVersion.ToString(3);

            WriteLine($@"Captura v{version}
(c) {DateTime.Now.Year} Mathew Sachin
");
        }
    }
}
