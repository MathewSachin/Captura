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
                                if (startOptions.Cursor)
                                    Settings.Instance.IncludeCursor = true;

                                if (startOptions.Clicks)
                                    Settings.Instance.MouseClicks = true;

                                if (startOptions.Keys)
                                    Settings.Instance.KeyStrokes = true;
                                
                                if (startOptions.Delay > 0)
                                    Thread.Sleep(startOptions.Delay);

                                vm.StartRecording();

                                if (startOptions.Length > 0)
                                {
                                    int elapsed = 0;

                                    System.Console.Write(TimeSpan.Zero);

                                    while (elapsed++ < startOptions.Length)
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

                                Task.Run(async () => await vm.StopRecording()).Wait();
                            }

                            // ScreenShot and Exit (Command-line)
                            else if (options is ShotCmdOptions shotOptions)
                            {
                                if (shotOptions.Cursor)
                                    Settings.Instance.IncludeCursor = true;

                                vm.CaptureScreenShot();
                            }
                        }
                    });

                    if (!success)
                        System.Console.WriteLine(verbs.GetUsage());
                    break;

                default:
                    Process.Start(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Captura.UI.exe"), string.Join(" ", args));
                    break;
            }
        }
    }
}
