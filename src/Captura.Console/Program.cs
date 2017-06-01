using Captura.Models;
using Captura.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Captura.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Handle if args is empty
            switch (args.Length > 0 ? args[0] : "")
            {
                case "start":
                case "shot":
                    // Reset settings
                    Settings.Instance.Reset();
                    Settings.Instance.UpdateRequired = false;

                    var verbs = new VerbCmdOptions();

                    var success = CommandLine.Parser.Default.ParseArguments(args, verbs, (verb, options) =>
                    {
                        using (var vm = new MainViewModel())
                        {
                            ServiceProvider.Register<IRegionProvider>(ServiceName.RegionProvider, new FakeRegionProvider());

                            ServiceProvider.Register<IMessageProvider>(ServiceName.Message, new FakeMessageProvider());

                            ServiceProvider.Register<IWebCamProvider>(ServiceName.WebCam, new FakeWebCamProvider());

                            ServiceProvider.Register<ISystemTray>(ServiceName.SystemTray, new FakeSystemTray());

                            vm.Init(false, false, false, false);

                            // Start Recording (Command-line)
                            if (options is StartCmdOptions startOptions)
                            {
                                Start(vm, startOptions);
                            }

                            // ScreenShot and Exit (Command-line)
                            else if (options is ShotCmdOptions shotOptions)
                            {
                                Shot(vm, shotOptions);
                            }
                        }
                    });

                    if (!success)
                        System.Console.WriteLine(verbs.GetUsage());
                    break;

                // Launch UI passing arguments
                default:
                    Process.Start(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Captura.UI.exe"), string.Join(" ", args));
                    break;
            }
        }

        static void Shot(MainViewModel ViewModel, ShotCmdOptions ShotOptions)
        {
            if (ShotOptions.Cursor)
                Settings.Instance.IncludeCursor = true;

            ViewModel.CaptureScreenShot();
        }

        static void Start(MainViewModel ViewModel, StartCmdOptions StartOptions)
        {
            if (StartOptions.Cursor)
                Settings.Instance.IncludeCursor = true;

            if (StartOptions.Clicks)
                Settings.Instance.MouseClicks = true;

            if (StartOptions.Keys)
                Settings.Instance.KeyStrokes = true;

            if (StartOptions.Delay > 0)
                Thread.Sleep(StartOptions.Delay);

            ViewModel.StartRecording();

            if (StartOptions.Length > 0)
            {
                int elapsed = 0;

                System.Console.Write(TimeSpan.Zero);

                while (elapsed++ < StartOptions.Length)
                {
                    Thread.Sleep(1000);
                    System.Console.Write(new string('\b', 8) + TimeSpan.FromSeconds(elapsed));
                }

                System.Console.Write(new string('\b', 8));
            }
            else
            {
                var text = "Press q to quit";

                System.Console.Write(text);

                while (System.Console.ReadKey(true).KeyChar != 'q') ;

                System.Console.Write(new string('\b', text.Length));
            }

            Task.Run(async () => await ViewModel.StopRecording()).Wait();
        }
    }
}
