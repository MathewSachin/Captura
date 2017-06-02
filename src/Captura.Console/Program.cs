using Captura.Models;
using Captura.ViewModels;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
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
                            ServiceProvider.Register<IRegionProvider>(ServiceName.RegionProvider, FakeRegionProvider.Instance);

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

        static void HandleVideoSource(MainViewModel ViewModel, CommonCmdOptions CommonOptions)
        {
            // Desktop
            if (CommonOptions.Source == null || CommonOptions.Source == "desktop")
                return;

            // Region
            if (Regex.IsMatch(CommonOptions.Source, @"^\d+,\d+,\d+,\d+$"))
            {
                var rect = (Rectangle) MainViewModel.RectangleConverter.ConvertFromString(CommonOptions.Source);

                FakeRegionProvider.Instance.SelectedRegion = rect;
                ViewModel.VideoViewModel.SelectedVideoSourceKind = VideoSourceKind.Region;
            }

            // Screen
            else if (ScreenItem.Count > 1 && Regex.IsMatch(CommonOptions.Source, @"screen:\d+"))
            {
                var index = int.Parse(CommonOptions.Source.Substring(7));

                if (index < ScreenItem.Count)
                {
                    ViewModel.VideoViewModel.SelectedVideoSourceKind = VideoSourceKind.Screen;
                    ViewModel.VideoViewModel.SelectedVideoSource = ViewModel.VideoViewModel.AvailableVideoSources[index];
                }
            }

            // No Video for Start
            else if (CommonOptions is StartCmdOptions && CommonOptions.Source == "none")
            {
                ViewModel.VideoViewModel.SelectedVideoSourceKind = VideoSourceKind.NoVideo;
            }

            // Window for Screenshot
            else if (CommonOptions is ShotCmdOptions && Regex.IsMatch(CommonOptions.Source, @"win:\d+"))
            {
                var ptr = int.Parse(CommonOptions.Source.Substring(4));

                try
                {
                    var rect = new Screna.Window(new IntPtr(ptr)).Rectangle;

                    if (rect != Rectangle.Empty)
                    {
                        FakeRegionProvider.Instance.SelectedRegion = rect;
                        ViewModel.VideoViewModel.SelectedVideoSourceKind = VideoSourceKind.Region;
                    }
                }
                catch
                {
                    // Suppress Errors
                }
            }
        }

        static void Shot(MainViewModel ViewModel, ShotCmdOptions ShotOptions)
        {
            if (ShotOptions.Cursor)
                Settings.Instance.IncludeCursor = true;

            HandleVideoSource(ViewModel, ShotOptions);

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

            HandleVideoSource(ViewModel, StartOptions);

            if (!ViewModel.RecordCommand.CanExecute(null))
            {
                System.Console.WriteLine("Nothing to Record");

                return;
            }

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
