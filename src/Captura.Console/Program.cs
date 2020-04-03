using System;
using System.Collections.Generic;
using System.Linq;
using Captura.Fakes;
using Captura.Native;
using CommandLine;
using static System.Console;
// ReSharper disable LocalizableElement

namespace Captura
{
    static class Program
    {
        [STAThread]
        static void Main(string[] Args)
        {
            User32.SetProcessDPIAware();

            ServiceProvider.LoadModule(new CoreModule());
            ServiceProvider.LoadModule(new FakesModule());
            ServiceProvider.LoadModule(new VerbsModule());

            var verbTypes = ServiceProvider
                .Get<IEnumerable<ICmdlineVerb>>()
                .Select(M => M.GetType())
                .ToArray();

            Parser.Default.ParseArguments(Args, verbTypes)
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
