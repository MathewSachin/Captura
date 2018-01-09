using Captura.Models;
using Captura.ViewModels;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Screna;
using static System.Console;
// ReSharper disable LocalizableElement

namespace Captura.Console
{
    static class Program
    {
        static void Main(string[] Args)
        {
            // Handle if args is empty
            switch (Args.Length > 0 ? Args[0] : "")
            {
                case "start":
                case "shot":
                case "ffmpeg":
                    Banner();

                    // Reset settings
                    Settings.Instance.SafeReset();

                    var verbs = new VerbCmdOptions();

                    if (!CommandLine.Parser.Default.ParseArguments(Args, verbs, (Verb, Options) =>
                    {
                        using (var vm = new MainViewModel())
                        {
                            RegisterFakes();

                            vm.Init(false, false, false, false);

                            // Remove Custom overlays
                            CustomOverlaysViewModel.Instance.Collection.Clear();

                            // Start Recording (Command-line)
                            switch (Options)
                            {
                                case StartCmdOptions startOptions:
                                    Start(vm, startOptions);
                                    break;

                                case ShotCmdOptions shotOptions:
                                    Shot(vm, shotOptions);
                                    break;

                                case FFMpegCmdOptions ffmpegOptions:
                                    FFMpeg(vm, ffmpegOptions);
                                    break;
                            }
                        }
                    }))
                    {
                        WriteLine("Invalid Arguments");
                    }
                    break;

                case "list":
                    List();
                    break;

                // Launch UI passing arguments
                default:
                    Process.Start(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Captura.UI.exe"), string.Join(" ", Args));
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

                var underline = $"\n{new string('-', 30)}";

                var video = vm.VideoViewModel;

                #region FFmpeg
                var ffmpegExists = FFMpegService.FFMpegExists;

                WriteLine($"FFmpeg Available: {(ffmpegExists ? "YES" : "NO")}");

                WriteLine();                

                if (ffmpegExists)
                {
                    WriteLine("FFmpeg ENCODERS" + underline);

                    video.SelectedVideoWriterKind = VideoWriterKind.FFMpeg;

                    for (var i = 0; i < video.AvailableVideoWriters.Count; ++i)
                    {
                        WriteLine($"{i.ToString().PadRight(2)}: {video.AvailableVideoWriters[i]}");
                    }

                    WriteLine();
                }
                #endregion

                #region SharpAvi
                var sharpAviExists = ServiceProvider.FileExists("SharpAvi.dll");

                WriteLine($"SharpAvi Available: {(sharpAviExists ? "YES" : "NO")}");

                WriteLine();

                if (sharpAviExists)
                {
                    WriteLine("SharpAvi ENCODERS" + underline);

                    video.SelectedVideoWriterKind = VideoWriterKind.SharpAvi;

                    for (var i = 0; i < video.AvailableVideoWriters.Count; ++i)
                    {
                        WriteLine($"{i.ToString().PadRight(2)}: {video.AvailableVideoWriters[i]}");
                    }

                    WriteLine();
                }
                #endregion

                #region Windows
                WriteLine("AVAILABLE WINDOWS" + underline);

                video.SelectedVideoSourceKind = VideoSourceKind.Window;

                foreach (var source in video.AvailableVideoSources.OfType<WindowItem>())
                {
                    WriteLine($"{source.Window.Handle.ToString().PadRight(10)}: {source}");
                }

                WriteLine();
                #endregion

                #region Screens
                WriteLine("AVAILABLE SCREENS" + underline);

                video.SelectedVideoSourceKind = VideoSourceKind.Screen;

                for (int i = 0; i < video.AvailableVideoSources.Count; ++i)
                {
                    WriteLine($"{i.ToString().PadRight(2)}: {video.AvailableVideoSources[i]}");
                }

                WriteLine();
                #endregion

                #region MouseKeyHook
                WriteLine($"MouseKeyHook Available: {(vm.MouseKeyHookAvailable ? "YES" : "NO")}");

                WriteLine();
                #endregion

                var audio = vm.AudioViewModel.AudioSource;

                WriteLine($"ManagedBass Available: {(audio is BassAudioSource ? "YES" : "NO")}");

                WriteLine();

                #region Microphones
                if (audio.AvailableRecordingSources.Count > 1)
                {
                    WriteLine("AVAILABLE MICROPHONES" + underline);

                    for (int i = 1; i < audio.AvailableRecordingSources.Count; ++i)
                    {
                        WriteLine($"{(i - 1).ToString().PadRight(2)}: {audio.AvailableRecordingSources[i]}");
                    }

                    WriteLine();
                }
                #endregion

                #region Speaker
                if (audio.AvailableLoopbackSources.Count > 1)
                {
                    WriteLine("AVAILABLE SPEAKER SOURCES" + underline);

                    for (int i = 1; i < audio.AvailableLoopbackSources.Count; ++i)
                    {
                        WriteLine($"{(i - 1).ToString().PadRight(2)}: {audio.AvailableLoopbackSources[i]}");
                    }

                    WriteLine();
                }
                #endregion
            }
        }

        static void RegisterFakes()
        {
            ServiceProvider.RegionProvider = FakeRegionProvider.Instance;

            ServiceProvider.MessageProvider = new FakeMessageProvider();

            ServiceProvider.WebCamProvider = new FakeWebCamProvider();

            ServiceProvider.SystemTray = new FakeSystemTray();

            ServiceProvider.MainWindow = new FakeWindowProvider();
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

            var video = ViewModel.VideoViewModel;

            // Region
            if (Regex.IsMatch(CommonOptions.Source, @"^\d+,\d+,\d+,\d+$"))
            {
                if (MainViewModel.RectangleConverter.ConvertFromInvariantString(CommonOptions.Source) is Rectangle rect)
                {
                    FakeRegionProvider.Instance.SelectedRegion = rect.Even();
                    video.SelectedVideoSourceKind = VideoSourceKind.Region;
                }
            }

            // Screen
            else if (Regex.IsMatch(CommonOptions.Source, @"^screen:\d+$"))
            {
                var index = int.Parse(CommonOptions.Source.Substring(7));

                if (index < ScreenItem.Count)
                {
                    video.SelectedVideoSourceKind = VideoSourceKind.Screen;

                    // First item is Full Screen
                    video.SelectedVideoSource = video.AvailableVideoSources[index + 1];
                }
            }

            // Desktop Duplication
            else if (CommonOptions is StartCmdOptions && Regex.IsMatch(CommonOptions.Source, @"^deskdupl:\d+$"))
            {
                var index = int.Parse(CommonOptions.Source.Substring(9));

                if (index < ScreenItem.Count)
                {
                    video.SelectedVideoSourceKind = VideoSourceKind.DesktopDuplication;

                    video.SelectedVideoSource = video.AvailableVideoSources[index];
                }
            }

            // No Video for Start
            else if (CommonOptions is StartCmdOptions && CommonOptions.Source == "none")
            {
                video.SelectedVideoSourceKind = VideoSourceKind.NoVideo;
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

        static void HandleVideoEncoder(MainViewModel ViewModel, StartCmdOptions StartOptions)
        {
            if (StartOptions.Encoder == null)
                return;

            var video = ViewModel.VideoViewModel;

            // FFMpeg
            if (FFMpegService.FFMpegExists && Regex.IsMatch(StartOptions.Encoder, @"^ffmpeg:\d+$"))
            {
                var index = int.Parse(StartOptions.Encoder.Substring(7));
                
                video.SelectedVideoWriterKind = VideoWriterKind.FFMpeg;

                if (index < video.AvailableVideoSources.Count)
                    video.SelectedVideoWriter = video.AvailableVideoWriters[index];
            }

            // SharpAvi
            else if (ServiceProvider.FileExists("SharpAvi.dll") && Regex.IsMatch(StartOptions.Encoder, @"^sharpavi:\d+$"))
            {
                var index = int.Parse(StartOptions.Encoder.Substring(9));

                video.SelectedVideoWriterKind = VideoWriterKind.SharpAvi;

                if (index < video.AvailableVideoSources.Count)
                    video.SelectedVideoWriter = video.AvailableVideoWriters[index];
            }

            // Gif
            else if (StartOptions.Encoder == "gif")
            {
                video.SelectedVideoWriterKind = VideoWriterKind.Gif;
            }
        }

        static void FFMpeg(MainViewModel ViewModel, FFMpegCmdOptions FFMpegOptions)
        {
            if (FFMpegOptions.install != null)
            {
                var downloadFolder = FFMpegOptions.install;

                if (!Directory.Exists(downloadFolder))
                {
                    WriteLine("Directory doesn't exist");
                    return;
                }

                var ffMpegDownload = new FFMpegDownloadViewModel
                {
                    TargetFolder = FFMpegOptions.install
                };

                var downloadTask = Task.Run(() => ffMpegDownload.Start());

                downloadTask.Wait();

                WriteLine(ffMpegDownload.Status);
            }
        }

        static void Shot(MainViewModel ViewModel, ShotCmdOptions ShotOptions)
        {
            if (ShotOptions.Cursor)
                Settings.Instance.IncludeCursor = true;

            // Screenshot Window with Transparency
            if (ShotOptions.Source != null && Regex.IsMatch(ShotOptions.Source, @"win:\d+"))
            {
                var ptr = int.Parse(ShotOptions.Source.Substring(4));

                try
                {
                    var bmp = ViewModel.ScreenShotWindow(new Window(new IntPtr(ptr)));

                    ViewModel.SaveScreenShot(bmp, ShotOptions.FileName);
                }
                catch
                {
                    // Suppress Errors
                }
            }
            else
            {
                HandleVideoSource(ViewModel, ShotOptions);

                ViewModel.CaptureScreenShot(ShotOptions.FileName);
            }
        }

        static void Start(MainViewModel ViewModel, StartCmdOptions StartOptions)
        {
            if (StartOptions.Cursor)
                Settings.Instance.IncludeCursor = true;

            if (StartOptions.Clicks)
                Settings.Instance.Clicks.Display = true;

            if (StartOptions.Keys)
                Settings.Instance.Keystrokes.Display = true;

            if (File.Exists(StartOptions.FileName))
            {
                WriteLine("Output File Already Exists");

                return;
            }

            HandleVideoSource(ViewModel, StartOptions);

            HandleVideoEncoder(ViewModel, StartOptions);

            HandleAudioSource(ViewModel, StartOptions);

            Settings.Instance.FrameRate = StartOptions.FrameRate;

            Settings.Instance.AudioQuality = StartOptions.AudioQuality;
            Settings.Instance.VideoQuality = StartOptions.VideoQuality;

            if (!ViewModel.RecordCommand.CanExecute(null))
            {
                WriteLine("Nothing to Record");

                return;
            }

            if (StartOptions.Delay > 0)
                Thread.Sleep(StartOptions.Delay);

            ViewModel.StartRecording(StartOptions.FileName);

            if (StartOptions.Length > 0)
            {
                var elapsed = 0;

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
                const string recordingText = "Press p to pause or resume, q to quit";

                WriteLine(recordingText);

                char ReadChar()
                {
                    if (IsInputRedirected)
                    {
                        var line = ReadLine();

                        if (line != null && line.Length == 1)
                            return line[0];

                        return char.MinValue;
                    }

                    return char.ToLower(ReadKey(true).KeyChar);
                }

                char c;

                do
                {
                    c = ReadChar();

                    if (c != 'p')
                        continue;
                    
                    ViewModel.PauseCommand.ExecuteIfCan();

                    if (ViewModel.RecorderState != RecorderState.Paused)
                    {
                        WriteLine("Resumed");
                    }
                } while (c != 'q');
            }

            Task.Run(async () => await ViewModel.StopRecording()).Wait();
        }
    }
}
