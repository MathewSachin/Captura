using Captura.Models;
using Captura.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Captura.Native;
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
                var uiPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "captura.exe");

                if (File.Exists(uiPath))
                {
                    Process.Start(uiPath);

                    return;
                }
            }

            User32.SetProcessDPIAware();

            ServiceProvider.LoadModule(new CoreModule());
            ServiceProvider.LoadModule(new FakesModule());

            // Hide on Full Screen Screenshot doesn't work on Console
            ServiceProvider.Get<Settings>().UI.HideOnFullScreenShot = false;

            Parser.Default.ParseArguments<StartCmdOptions, ShotCmdOptions, FFmpegCmdOptions, ListCmdOptions>(Args)
                .WithParsed((Action<ListCmdOptions>)(Options => List()))
                .WithParsed((Action<StartCmdOptions>)(Options =>
                {
                    Banner();

                    using (var vm = ServiceProvider.Get<MainViewModel>())
                    {
                        CopySettings(vm.Settings);

                        Start(vm, Options);
                    }
                }))
                .WithParsed((Action<ShotCmdOptions>)(Options =>
                {
                    Banner();

                    using (var vm = ServiceProvider.Get<MainViewModel>())
                    {
                        Shot(vm, Options);
                    }
                }))
                .WithParsed((Action<FFmpegCmdOptions>)(Options =>
                {
                    Banner();

                    FFmpeg(Options);
                }));
        }

        static void CopySettings(Settings Settings)
        {
            // Load settings dummy
            var dummySettings = new Settings();
            dummySettings.Load();

            Settings.WebcamOverlay = dummySettings.WebcamOverlay;
            Settings.MousePointerOverlay = dummySettings.MousePointerOverlay;
            Settings.Clicks = dummySettings.Clicks;
            Settings.Keystrokes = dummySettings.Keystrokes;
            Settings.Elapsed = dummySettings.Elapsed;

            // FFmpeg Path
            Settings.FFmpeg.FolderPath = dummySettings.FFmpeg.FolderPath;

            foreach (var overlay in dummySettings.Censored)
            {
                Settings.Censored.Add(overlay);
            }

            foreach (var overlay in dummySettings.TextOverlays)
            {
                Settings.TextOverlays.Add(overlay);
            }

            foreach (var overlay in dummySettings.ImageOverlays)
            {
                Settings.ImageOverlays.Add(overlay);
            }
        }

        static void List()
        {
            Banner();

            var underline = $"\n{new string('-', 30)}";

            #region FFmpeg
            var ffmpegExists = FFmpegService.FFmpegExists;

            WriteLine($"FFmpeg Available: {(ffmpegExists ? "YES" : "NO")}");

            WriteLine();

            if (ffmpegExists)
            {
                WriteLine("FFmpeg ENCODERS" + underline);

                var writerProvider = ServiceProvider.Get<FFmpegWriterProvider>();

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

            // Window Picker is skipped automatically
            foreach (var source in Window.EnumerateVisible())
            {
                WriteLine($"{source.Handle.ToString().PadRight(10)}: {source.Title}");
            }

            WriteLine();
            #endregion

            #region Screens
            WriteLine("AVAILABLE SCREENS" + underline);

            var j = 0;

            // First is Full Screen, Second is Screen Picker
            foreach (var screen in ScreenItem.Enumerate())
            {
                WriteLine($"{j.ToString().PadRight(2)}: {screen.Name}");

                ++j;
            }

            WriteLine();
            #endregion

            var audio = ServiceProvider.Get<AudioSource>();

            WriteLine($"ManagedBass Available: {(audio is BassAudioSource ? "YES" : "NO")}");

            WriteLine();

            #region Microphones
            if (audio.AvailableRecordingSources.Count > 0)
            {
                WriteLine("AVAILABLE MICROPHONES" + underline);

                for (var i = 0; i < audio.AvailableRecordingSources.Count; ++i)
                {
                    WriteLine($"{i.ToString().PadRight(2)}: {audio.AvailableRecordingSources[i]}");
                }

                WriteLine();
            }
            #endregion

            #region Speaker
            if (audio.AvailableLoopbackSources.Count > 0)
            {
                WriteLine("AVAILABLE SPEAKER SOURCES" + underline);

                for (var i = 0; i < audio.AvailableLoopbackSources.Count; ++i)
                {
                    WriteLine($"{i.ToString().PadRight(2)}: {audio.AvailableLoopbackSources[i]}");
                }

                WriteLine();
            }
            #endregion

            #region Webcams
            var webcam = ServiceProvider.Get<IWebCamProvider>();

            if (webcam.AvailableCams.Count > 1)
            {
                WriteLine("AVAILABLE WEBCAMS" + underline);

                for (var i = 1; i < webcam.AvailableCams.Count; ++i)
                {
                    WriteLine($"{(i - 1).ToString().PadRight(2)}: {webcam.AvailableCams[i]}");
                }

                WriteLine();
            }
            #endregion
        }

        static void Banner()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

            WriteLine($@"Captura v{version}
(c) {DateTime.Now.Year} Mathew Sachin
");
        }

        static void HandleVideoSource(VideoSourcesViewModel VideoSourcesViewModel, CommonCmdOptions CommonOptions)
        {
            if (CommonOptions.Source == null)
                return;

            var providers = ServiceProvider.Get<IEnumerable<IVideoSourceProvider>>();

            var provider = providers.FirstOrDefault(M => M.ParseCli(CommonOptions.Source));

            if (provider != null)
            {
                VideoSourcesViewModel.RestoreSourceKind(provider);
            }
        }

        static void HandleAudioSource(AudioSettings Settings, StartCmdOptions StartOptions)
        {
            var audioSource = ServiceProvider.Get<AudioSource>();

            if (StartOptions.Microphone != -1 && StartOptions.Microphone < audioSource.AvailableRecordingSources.Count)
            {
                Settings.Enabled = true;
                audioSource.AvailableRecordingSources[StartOptions.Microphone].Active = true;
            }

            if (StartOptions.Speaker != -1 && StartOptions.Speaker < audioSource.AvailableLoopbackSources.Count)
            {
                Settings.Enabled = true;
                audioSource.AvailableLoopbackSources[StartOptions.Speaker].Active = true;
            }
        }

        static void HandleVideoEncoder(StartCmdOptions StartOptions)
        {
            var videoWritersViewModel = ServiceProvider.Get<VideoWritersViewModel>();

            if (StartOptions.Encoder == null)
                return;

            // FFmpeg
            if (FFmpegService.FFmpegExists && Regex.IsMatch(StartOptions.Encoder, @"^ffmpeg:\d+$"))
            {
                var index = int.Parse(StartOptions.Encoder.Substring(7));

                videoWritersViewModel.SelectedVideoWriterKind = ServiceProvider.Get<FFmpegWriterProvider>();

                if (index < videoWritersViewModel.AvailableVideoWriters.Count)
                    videoWritersViewModel.SelectedVideoWriter = videoWritersViewModel.AvailableVideoWriters[index];
            }

            // SharpAvi
            else if (ServiceProvider.FileExists("SharpAvi.dll") && Regex.IsMatch(StartOptions.Encoder, @"^sharpavi:\d+$"))
            {
                var index = int.Parse(StartOptions.Encoder.Substring(9));

                videoWritersViewModel.SelectedVideoWriterKind = ServiceProvider.Get<SharpAviWriterProvider>();

                if (index < videoWritersViewModel.AvailableVideoWriters.Count)
                    videoWritersViewModel.SelectedVideoWriter = videoWritersViewModel.AvailableVideoWriters[index];
            }

            // Gif
            else if (StartOptions.Encoder == "gif")
            {
                videoWritersViewModel.SelectedVideoWriterKind = ServiceProvider.Get<GifWriterProvider>();
            }
        }

        static void HandleWebcam(StartCmdOptions StartOptions)
        {
            var webcam = ServiceProvider.Get<IWebCamProvider>();

            if (StartOptions.Webcam != -1 && StartOptions.Webcam < webcam.AvailableCams.Count - 1)
            {
                webcam.SelectedCam = webcam.AvailableCams[StartOptions.Webcam + 1];

                // Sleep to prevent AccessViolationException
                Thread.Sleep(500);
            }
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

        static void Shot(MainViewModel ViewModel, ShotCmdOptions ShotOptions)
        {
            ViewModel.Settings.IncludeCursor = ShotOptions.Cursor;

            // Screenshot Window with Transparency
            if (ShotOptions.Source != null && Regex.IsMatch(ShotOptions.Source, @"win:\d+"))
            {
                var ptr = int.Parse(ShotOptions.Source.Substring(4));

                try
                {
                    var bmp = ViewModel.ScreenShotViewModel.ScreenShotWindow(new Window(new IntPtr(ptr)));

                    ViewModel.ScreenShotViewModel.SaveScreenShot(bmp, ShotOptions.FileName).Wait();
                }
                catch
                {
                    // Suppress Errors
                }
            }
            else
            {
                HandleVideoSource(ViewModel.VideoSourcesViewModel, ShotOptions);

                ViewModel.ScreenShotViewModel.CaptureScreenShot(ShotOptions.FileName);
            }
        }

        static void Start(MainViewModel ViewModel, StartCmdOptions StartOptions)
        {
            var settings = ViewModel.Settings;

            settings.IncludeCursor = StartOptions.Cursor;
            settings.Clicks.Display = StartOptions.Clicks;
            settings.Keystrokes.Display = StartOptions.Keys;

            if (File.Exists(StartOptions.FileName))
            {
                if (!StartOptions.Overwrite)
                {
                    if (!ServiceProvider.MessageProvider
                        .ShowYesNo("Output File Already Exists, Do you want to overwrite?", ""))
                        return;
                }

                File.Delete(StartOptions.FileName);
            }

            HandleVideoSource(ViewModel.VideoSourcesViewModel, StartOptions);

            HandleVideoEncoder(StartOptions);

            HandleAudioSource(settings.Audio, StartOptions);

            HandleWebcam(StartOptions);

            if (StartOptions.FrameRate is int frameRate)
                settings.Video.FrameRate = frameRate;

            if (StartOptions.AudioQuality is int aq)
                settings.Audio.Quality = aq;

            if (StartOptions.VideoQuality is int vq)
                settings.Video.Quality = vq;

            if (!ViewModel.RecordingViewModel.RecordCommand.CanExecute(null))
            {
                WriteLine("Nothing to Record");

                return;
            }

            if (StartOptions.Delay > 0)
                Thread.Sleep(StartOptions.Delay);

            if (!ViewModel.RecordingViewModel.StartRecording(StartOptions.FileName))
                return;

            Task.Factory.StartNew(() =>
            {
                Loop(ViewModel, StartOptions);

                ViewModel.RecordingViewModel.StopRecording().Wait();

                Application.Exit();
            });

            // MouseKeyHook requires a Window Handle to register
            Application.Run(new ApplicationContext());
        }

        static void Loop(MainViewModel ViewModel, StartCmdOptions StartOptions)
        {
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

                    ViewModel.RecordingViewModel.PauseCommand.ExecuteIfCan();

                    if (ViewModel.RecordingViewModel.RecorderState != RecorderState.Paused)
                    {
                        WriteLine("Resumed");
                    }
                } while (c != 'q');
            }
        }
    }
}
