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

            Parser.Default.ParseArguments<StartCmdOptions, ShotCmdOptions, FFmpegCmdOptions, ListCmdOptions, UploadCmdOptions>(Args)
                .WithParsed((object Options) =>
                {
                    // Always display Banner
                    Banner();

                    switch (Options)
                    {
                        case ListCmdOptions _:
                            var lister = ServiceProvider.Get<ConsoleLister>();

                            lister.List();
                            break;

                        case StartCmdOptions start:
                            using (var manager = ServiceProvider.Get<ConsoleManager>())
                            {
                                manager.CopySettings();

                                manager.Start(start);
                            }
                            break;

                        case ShotCmdOptions shot:
                            using (var manager = ServiceProvider.Get<ConsoleManager>())
                            {
                                manager.Shot(shot);
                            }
                            break;

                        case FFmpegCmdOptions ffmpeg:
                            var ffmpegManager = ServiceProvider.Get<FFmpegConsoleManager>();

                            // Need to Wait instead of await otherwise the process will exit
                            ffmpegManager.Run(ffmpeg).Wait();
                            break;

                        case UploadCmdOptions upload:
                            Upload(upload);
                            break;
                    }
                });
        }

        static void Banner()
        {
            var version = ServiceProvider.AppVersion.ToString(3);

            WriteLine($@"Captura v{version}
(c) {DateTime.Now.Year} Mathew Sachin
");
        }

        static void Upload(UploadCmdOptions Options)
        {
            if (!File.Exists(Options.FileName))
            {
                WriteLine("File not found");
                return;
            }

            switch (Options.Service)
            {
                case UploadService.imgur:
                    var imgSystem = ServiceProvider.Get<IImagingSystem>();
                    var img = imgSystem.LoadBitmap(Options.FileName);
                    var uploader = ServiceProvider.Get<IImageUploader>();

                    // TODO: Show progress (on a single line)
                    var result = uploader.Upload(img, ImageFormats.Png, P => { }).Result;

                    WriteLine(result.Url);
                    break;
            }
        }
    }
}
