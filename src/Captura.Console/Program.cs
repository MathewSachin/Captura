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
using static System.Console;

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
                    Banner();

                    // Reset settings
                    Settings.Instance.Reset();
                    Settings.Instance.UpdateRequired = false;

                    var verbs = new VerbCmdOptions();

                    var success = CommandLine.Parser.Default.ParseArguments(args, verbs, (verb, options) =>
                    {
                        using (var vm = new MainViewModel())
                        {
                            RegisterFakes();

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
                        WriteLine(verbs.GetUsage());
                    break;

                case "list":
                    List();
                    break;

                // Launch UI passing arguments
                default:
                    Process.Start(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Captura.UI.exe"), string.Join(" ", args));
                    break;
            }
        }

        static void List()
        {
            Banner();

            using (var vm = new MainViewModel())
            {
                RegisterFakes();

                vm.Init(false, false, false, false);

                WriteLine($"FFmpeg Available: {(ServiceProvider.FFMpegExists ? "YES" : "NO")}");

                WriteLine();

                var underline = $"\n{new string('-', 30)}";

                WriteLine("AVAILABLE WINDOWS" + underline);

                vm.VideoViewModel.SelectedVideoSourceKind = VideoSourceKind.Window;

                foreach (var source in vm.VideoViewModel.AvailableVideoSources)
                {
                    WriteLine($"{(source as WindowItem).Window.Handle.ToString().PadRight(10)}: {source}");
                }

                WriteLine();

                var audio = vm.AudioViewModel.AudioSource;

                if (audio.AvailableRecordingSources.Count > 1)
                {
                    WriteLine("AVAILABLE MICROPHONES" + underline);

                    for (int i = 1; i < audio.AvailableRecordingSources.Count; ++i)
                    {
                        WriteLine($"{(i - 1).ToString().PadRight(2)}: {audio.AvailableRecordingSources[i]}");
                    }

                    WriteLine();
                }

                if (audio.AvailableLoopbackSources.Count > 1)
                {
                    WriteLine("AVAILABLE SPEAKER SOURCES" + underline);

                    for (int i = 1; i < audio.AvailableLoopbackSources.Count; ++i)
                    {
                        WriteLine($"{(i - 1).ToString().PadRight(2)}: {audio.AvailableLoopbackSources[i]}");
                    }

                    WriteLine();
                }
            }
        }

        static void RegisterFakes()
        {
            ServiceProvider.Register<IRegionProvider>(ServiceName.RegionProvider, FakeRegionProvider.Instance);

            ServiceProvider.Register<IMessageProvider>(ServiceName.Message, new FakeMessageProvider());

            ServiceProvider.Register<IWebCamProvider>(ServiceName.WebCam, new FakeWebCamProvider());

            ServiceProvider.Register<ISystemTray>(ServiceName.SystemTray, new FakeSystemTray());
        }

        static void Banner()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

            WriteLine($@"Captura v{version}
(c) 2017 Mathew Sachin
");
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

        static void HandleAudioSource(MainViewModel ViewModel, StartCmdOptions StartOptions)
        {
            var source = ViewModel.AudioViewModel.AudioSource;

            if (StartOptions.Microphone != -1 && StartOptions.Microphone < source.AvailableRecordingSources.Count - 1)
            {
                source.SelectedRecordingSource = source.AvailableRecordingSources[StartOptions.Microphone + 1];
            }

            if (StartOptions.Speaker != -1 && StartOptions.Speaker < source.AvailableLoopbackSources.Count - 1)
            {
                source.SelectedLoopbackSource = source.AvailableLoopbackSources[StartOptions.Speaker + 1];
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

            HandleAudioSource(ViewModel, StartOptions);

            if (!ViewModel.RecordCommand.CanExecute(null))
            {
                WriteLine("Nothing to Record");

                return;
            }

            if (StartOptions.Delay > 0)
                Thread.Sleep(StartOptions.Delay);

            ViewModel.StartRecording();

            if (StartOptions.Length > 0)
            {
                int elapsed = 0;

                Write(TimeSpan.Zero);

                while (elapsed++ < StartOptions.Length)
                {
                    Thread.Sleep(1000);
                    Write(new string('\b', 8) + TimeSpan.FromSeconds(elapsed));
                }

                Write(new string('\b', 8));
            }
            else
            {
                var text = "Press q to quit";

                Write(text);

                while (ReadKey(true).KeyChar != 'q') ;

                Write(new string('\b', text.Length));
            }

            Task.Run(async () => await ViewModel.StopRecording()).Wait();
        }
    }
}
