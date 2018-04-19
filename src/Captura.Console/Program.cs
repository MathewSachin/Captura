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
using CommandLine;
using Screna;
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
                Process.Start(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "Captura.UI.exe"));

                return;
            }

            // Hide on Full Screen Screenshot doesn't work on Console
            ServiceProvider.Get<Settings>().UI.HideOnFullScreenShot = false;
            
            ServiceProvider.LoadModule(new FakesModule());

            Parser.Default.ParseArguments<StartCmdOptions, ShotCmdOptions, FFMpegCmdOptions, ListCmdOptions>(Args)
                .WithParsed<ListCmdOptions>(Options => List())
                .WithParsed<StartCmdOptions>(Options =>
                {
                    Banner();

                    using (var vm = ServiceProvider.Get<MainViewModel>())
                    {
                        vm.Init(false, false, false, false);

                        // Remove Custom overlays
                        vm.CustomOverlays.Reset();
                        vm.CustomImageOverlays.Reset();

                        Start(vm, Options);
                    }
                })
                .WithParsed<ShotCmdOptions>(Options =>
                {
                    Banner();

                    using (var vm = ServiceProvider.Get<MainViewModel>())
                    {
                        vm.Init(false, false, false, false);

                        Shot(vm, Options);
                    }
                })
                .WithParsed<FFMpegCmdOptions>(Options =>
                {
                    Banner();

                    FFMpeg(Options);
                });
        }

        static void List()
        {
            Banner();

            var underline = $"\n{new string('-', 30)}";

            #region FFmpeg
            var ffmpegExists = FFMpegService.FFMpegExists;

            WriteLine($"FFmpeg Available: {(ffmpegExists ? "YES" : "NO")}");

            WriteLine();

            if (ffmpegExists)
            {
                WriteLine("FFmpeg ENCODERS" + underline);

                var writerProvider = ServiceProvider.Get<FFMpegWriterProvider>();

                var i = 0;

                foreach (var codec in writerProvider)
                {
                    WriteLine($"{i.ToString().PadRight(2)}: {codec}");
                    ++i;
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

                var writerProvider = ServiceProvider.Get<SharpAviWriterProvider>();

                var i = 0;

                foreach (var codec in writerProvider)
                {
                    WriteLine($"{i.ToString().PadRight(2)}: {codec}");
                    ++i;
                }

                WriteLine();
            }
            #endregion

            #region Windows
            WriteLine("AVAILABLE WINDOWS" + underline);

            var winProvider = ServiceProvider.Get<WindowSourceProvider>();

            foreach (var source in winProvider.OfType<WindowItem>())
            {
                WriteLine($"{source.Window.Handle.ToString().PadRight(10)}: {source}");
            }

            WriteLine();
            #endregion

            #region Screens
            WriteLine("AVAILABLE SCREENS" + underline);

            var scrProvider = ServiceProvider.Get<ScreenSourceProvider>();

            var j = 0;

            // First is Full Screen
            foreach (var screen in scrProvider.Skip(1))
            {
                WriteLine($"{j.ToString().PadRight(2)}: {screen}");

                ++j;
            }

            WriteLine();
            #endregion

            #region MouseKeyHook
            WriteLine($"MouseKeyHook Available: {(ServiceProvider.Get<MainViewModel>().MouseKeyHookAvailable ? "YES" : "NO")}");

            WriteLine();
            #endregion

            var audio = ServiceProvider.Get<AudioSource>();

            WriteLine($"ManagedBass Available: {(audio is BassAudioSource ? "YES" : "NO")}");

            WriteLine();

            #region Microphones
            if (audio.AvailableRecordingSources.Count > 1)
            {
                WriteLine("AVAILABLE MICROPHONES" + underline);

                for (var i = 1; i < audio.AvailableRecordingSources.Count; ++i)
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

                for (var i = 1; i < audio.AvailableLoopbackSources.Count; ++i)
                {
                    WriteLine($"{(i - 1).ToString().PadRight(2)}: {audio.AvailableLoopbackSources[i]}");
                }

                WriteLine();
            }
            #endregion
        }

        static void Banner()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

            WriteLine($@"Captura v{version}
(c) 2018 Mathew Sachin
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
                    video.SelectedVideoSourceKind = ServiceProvider.Get<RegionSourceProvider>();
                }
            }

            // Screen
            else if (Regex.IsMatch(CommonOptions.Source, @"^screen:\d+$"))
            {
                var index = int.Parse(CommonOptions.Source.Substring(7));

                if (index < ScreenItem.Count)
                {
                    video.SelectedVideoSourceKind = ServiceProvider.Get<ScreenSourceProvider>();

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
                    video.SelectedVideoSourceKind = ServiceProvider.Get<DeskDuplSourceProvider>();

                    video.SelectedVideoSource = video.AvailableVideoSources[index];
                }
            }

            // No Video for Start
            else if (CommonOptions is StartCmdOptions && CommonOptions.Source == "none")
            {
                video.SelectedVideoSourceKind = ServiceProvider.Get<NoVideoSourceProvider>();
            }            
        }

        static void HandleAudioSource(MainViewModel ViewModel, StartCmdOptions StartOptions)
        {
            var source = ViewModel.AudioSource;

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

                video.SelectedVideoWriterKind = ServiceProvider.Get<FFMpegWriterProvider>();

                if (index < video.AvailableVideoSources.Count)
                    video.SelectedVideoWriter = video.AvailableVideoWriters[index];
            }

            // SharpAvi
            else if (ServiceProvider.FileExists("SharpAvi.dll") && Regex.IsMatch(StartOptions.Encoder, @"^sharpavi:\d+$"))
            {
                var index = int.Parse(StartOptions.Encoder.Substring(9));

                video.SelectedVideoWriterKind = ServiceProvider.Get<SharpAviWriterProvider>();

                if (index < video.AvailableVideoSources.Count)
                    video.SelectedVideoWriter = video.AvailableVideoWriters[index];
            }

            // Gif
            else if (StartOptions.Encoder == "gif")
            {
                video.SelectedVideoWriterKind = ServiceProvider.Get<GifWriterProvider>();
            }
        }

        static async void FFMpeg(FFMpegCmdOptions FFMpegOptions)
        {
            if (FFMpegOptions.Install != null)
            {
                var downloadFolder = FFMpegOptions.Install;

                if (!Directory.Exists(downloadFolder))
                {
                    WriteLine("Directory doesn't exist");
                    return;
                }

                var ffMpegDownload = ServiceProvider.Get<FFMpegDownloadViewModel>();

                ffMpegDownload.TargetFolder = FFMpegOptions.Install;

                await ffMpegDownload.Start();
                
                WriteLine(ffMpegDownload.Status);
            }
        }

        static void Shot(MainViewModel ViewModel, ShotCmdOptions ShotOptions)
        {
            if (ShotOptions.Cursor)
                ViewModel.Settings.IncludeCursor = true;

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
                ViewModel.Settings.IncludeCursor = true;

            if (StartOptions.Clicks)
                ViewModel.Settings.Clicks.Display = true;

            if (StartOptions.Keys)
                ViewModel.Settings.Keystrokes.Display = true;

            if (File.Exists(StartOptions.FileName))
            {
                WriteLine("Output File Already Exists");

                return;
            }

            HandleVideoSource(ViewModel, StartOptions);

            HandleVideoEncoder(ViewModel, StartOptions);

            HandleAudioSource(ViewModel, StartOptions);

            ViewModel.Settings.Video.FrameRate = StartOptions.FrameRate;

            ViewModel.Settings.Audio.Quality = StartOptions.AudioQuality;
            ViewModel.Settings.Video.Quality = StartOptions.VideoQuality;

            if (!ViewModel.RecordCommand.CanExecute(null))
            {
                WriteLine("Nothing to Record");

                return;
            }

            if (StartOptions.Delay > 0)
                Thread.Sleep(StartOptions.Delay);

            if (!ViewModel.StartRecording(StartOptions.FileName))
                return;

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
