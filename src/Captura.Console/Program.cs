using Captura.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
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
            if (Args.Length == 0)
            {
                var uiPath = Path.Combine(ServiceProvider.AppDir, "captura.exe");

                if (File.Exists(uiPath))
                {
                    Process.Start(uiPath);

                    return;
                }
            }

            User32.SetProcessDPIAware();

            ServiceProvider.LoadModule(new CoreModule());
            ServiceProvider.LoadModule(new FakesModule());

            Parser.Default.ParseArguments<StartCmdOptions, ShotCmdOptions, FFmpegCmdOptions, ListCmdOptions>(Args)
                .WithParsed((ListCmdOptions Options) =>
                {
                    Banner();

                    var lister = ServiceProvider.Get<ConsoleLister>();

                    lister.List();
                })
                .WithParsed((StartCmdOptions Options) =>
                {
                    Banner();

                    using (var manager = ServiceProvider.Get<ConsoleManager>())
                    {
                        manager.CopySettings();

                        manager.Start(Options);
                    }
                })
                .WithParsed((ShotCmdOptions Options) =>
                {
                    Banner();

                    using (var manager = ServiceProvider.Get<ConsoleManager>())
                    {
                        manager.Shot(Options);
                    }
                })
                .WithParsed((FFmpegCmdOptions Options) =>
                {
                    Banner();

                    FFmpeg(Options);
                });
        }

        static void Banner()
        {
            var version = ServiceProvider.AppVersion.ToString(3);

            WriteLine($@"Captura v{version}
(c) {DateTime.Now.Year} Mathew Sachin
");
        }

        static async void FFmpeg(FFmpegCmdOptions FFmpegOptions)
        {
            if (FFmpegOptions.Install != null)
            {
                var downloadFolder = FFmpegOptions.Install;

                if (!Directory.Exists(downloadFolder))
                {
                    WriteLine("Directory doesn't exist");
                    return;
                }

                var ffMpegDownload = ServiceProvider.Get<FFmpegDownloadViewModel>();

                ffMpegDownload.TargetFolder = FFmpegOptions.Install;

                await ffMpegDownload.Start();
                
                WriteLine(ffMpegDownload.Status);
            }
        }
    }
}
